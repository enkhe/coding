// AutoML — let ML.NET pick the algorithm and tune.
// Package: Microsoft.ML.AutoML
using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;

namespace MLNetSamples;

public sealed class CustomerChurn
{
    [LoadColumn(0)] public float Tenure { get; set; }
    [LoadColumn(1)] public float MonthlyCharges { get; set; }
    [LoadColumn(2)] public string Contract { get; set; } = "";
    [LoadColumn(3)] public bool Churned { get; set; }
}

public static class AutoMlExample
{
    public static void Run()
    {
        var ml = new MLContext(seed: 1);
        var data = ml.Data.LoadFromTextFile<CustomerChurn>("./data/churn.csv", hasHeader: true, separatorChar: ',');

        var pipeline = ml.Auto()
            .Featurizer(data, "Features", excludeColumns: ["Churned"])
            .Append(ml.Auto().BinaryClassification(labelColumnName: "Churned"));

        var experiment = ml.Auto()
            .CreateExperiment()
            .SetPipeline(pipeline)
            .SetBinaryClassificationMetric(BinaryClassificationMetric.AreaUnderRocCurve, "Churned")
            .SetTrainingTimeInSeconds(120)
            .SetDataset(data, fold: 5);

        var trial = experiment.Run();
        Console.WriteLine($"Best AUC: {trial.Metric:F3}");
        ml.Model.Save(trial.Model, data.Schema, "./artifacts/churn-best.zip");
    }
}
