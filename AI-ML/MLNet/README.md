# ML.NET

> Native .NET ML — train, evaluate, deploy without leaving the .NET stack.

## Core Concepts

- **`MLContext`** — root for everything; deterministic if seeded.
- **`IDataView`** — schema-aware lazy data set.
- **Pipelines** — composed transformations + a trainer; `Fit()` returns a model.
- **Trainers** — classification, regression, clustering, ranking, forecasting, recommendation.
- **AutoML** — pick algo + hyperparameters automatically (`mlContext.Auto()`).
- **ONNX export** — for cross-language serving.

## "To Be Dangerous" Cheatsheet

| Task | Trainer |
|---|---|
| Binary classification | `BinaryClassification.Trainers.SdcaLogisticRegression()` |
| Multi-class classification | `MulticlassClassification.Trainers.SdcaMaximumEntropy()` |
| Regression | `Regression.Trainers.LightGbm()` |
| Clustering | `Clustering.Trainers.KMeans()` |
| Recommendation | `Recommendation.Trainers.MatrixFactorization()` |
| AutoML | `mlContext.Auto().BinaryClassification(...).Execute()` |

## Quick Reference

```csharp
var ml = new MLContext(seed: 1);

var data = ml.Data.LoadFromTextFile<TaxiTrip>("./data/trips.csv", hasHeader: true, separatorChar: ',');
var split = ml.Data.TrainTestSplit(data, testFraction: 0.2);

var pipeline = ml.Transforms.CopyColumns("Label", "FareAmount")
    .Append(ml.Transforms.Categorical.OneHotEncoding("PaymentTypeEncoded", "PaymentType"))
    .Append(ml.Transforms.Concatenate("Features",
        "PassengerCount", "TripDistance", "PaymentTypeEncoded"))
    .Append(ml.Regression.Trainers.LightGbm());

var model = pipeline.Fit(split.TrainSet);
var preds = model.Transform(split.TestSet);
var metrics = ml.Regression.Evaluate(preds, "Label");
Console.WriteLine($"R²: {metrics.RSquared:F3}, RMSE: {metrics.RootMeanSquaredError:F2}");

ml.Model.Save(model, split.TrainSet.Schema, "./taxi-fare.zip");
```

## Common Pitfalls

- Forgetting `seed` on `MLContext` → non-reproducible runs.
- Training on an unbalanced binary set without class-weighting.
- Saving `.zip` but not committing the schema → load-time mismatches.
- Using `LoadFromTextFile` on giant files in memory; use `LoadFromEnumerable` with streaming.

## Examples in this folder

- [`TaxiFareRegression.cs`](TaxiFareRegression.cs)
- [`AutoMlExample.cs`](AutoMlExample.cs)

## See also

- [../MachineLearning](../MachineLearning/) · [../MLOps](../MLOps/)
