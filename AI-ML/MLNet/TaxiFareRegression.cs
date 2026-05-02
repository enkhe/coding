// ML.NET 4 — taxi-fare regression. Train, evaluate, save, predict.
// Package: Microsoft.ML, Microsoft.ML.LightGbm
using Microsoft.ML;
using Microsoft.ML.Data;

namespace MLNetSamples;

public sealed class TaxiTrip
{
    [LoadColumn(0)] public string VendorId { get; set; } = "";
    [LoadColumn(1)] public string RateCode { get; set; } = "";
    [LoadColumn(2)] public float PassengerCount { get; set; }
    [LoadColumn(3)] public float TripTime { get; set; }
    [LoadColumn(4)] public float TripDistance { get; set; }
    [LoadColumn(5)] public string PaymentType { get; set; } = "";
    [LoadColumn(6)] public float FareAmount { get; set; }
}

public sealed class FarePrediction
{
    [ColumnName("Score")] public float PredictedFare { get; set; }
}

public static class TaxiFareRegression
{
    public static void Run()
    {
        var ml = new MLContext(seed: 1);

        var data = ml.Data.LoadFromTextFile<TaxiTrip>(
            "./data/trips.csv", hasHeader: true, separatorChar: ',');
        var split = ml.Data.TrainTestSplit(data, testFraction: 0.2);

        var pipeline = ml.Transforms.CopyColumns("Label", nameof(TaxiTrip.FareAmount))
            .Append(ml.Transforms.Categorical.OneHotEncoding("VendorEnc", nameof(TaxiTrip.VendorId)))
            .Append(ml.Transforms.Categorical.OneHotEncoding("RateEnc", nameof(TaxiTrip.RateCode)))
            .Append(ml.Transforms.Categorical.OneHotEncoding("PayEnc", nameof(TaxiTrip.PaymentType)))
            .Append(ml.Transforms.Concatenate("Features",
                "VendorEnc", "RateEnc", "PayEnc",
                nameof(TaxiTrip.PassengerCount),
                nameof(TaxiTrip.TripTime),
                nameof(TaxiTrip.TripDistance)))
            .Append(ml.Regression.Trainers.LightGbm(labelColumnName: "Label", featureColumnName: "Features"));

        var model = pipeline.Fit(split.TrainSet);
        var preds = model.Transform(split.TestSet);
        var metrics = ml.Regression.Evaluate(preds, "Label");

        Console.WriteLine($"R²: {metrics.RSquared:F3}");
        Console.WriteLine($"RMSE: {metrics.RootMeanSquaredError:F2}");
        Console.WriteLine($"MAE: {metrics.MeanAbsoluteError:F2}");

        ml.Model.Save(model, split.TrainSet.Schema, "./artifacts/taxi-fare.zip");

        // Single-prediction engine (thread-safe via PredictionEnginePool in ASP.NET Core).
        var engine = ml.Model.CreatePredictionEngine<TaxiTrip, FarePrediction>(model);
        var fare = engine.Predict(new TaxiTrip
        {
            VendorId = "VTS",
            RateCode = "1",
            PassengerCount = 1,
            TripTime = 1140,
            TripDistance = 3.75f,
            PaymentType = "CRD",
        });
        Console.WriteLine($"Predicted fare: ${fare.PredictedFare:F2}");
    }
}
