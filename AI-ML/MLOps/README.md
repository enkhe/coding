# MLOps

> Apply DevOps to models. Reproducible training, versioned artifacts, monitored deployments.

## "To Be Dangerous" Cheatsheet

| Need | Tool |
|---|---|
| Experiment tracking | **MLflow**, **Weights & Biases**, Neptune |
| Model registry | MLflow Model Registry, SageMaker, Vertex AI |
| Feature store | Feast, Tecton, Vertex AI Feature Store |
| Pipelines | **Kubeflow Pipelines**, Airflow, Azure ML Pipelines, Prefect |
| Serving | TorchServe, **Triton Inference Server**, KServe, FastAPI + ONNX |
| Drift / quality | Evidently, WhyLabs, Arize |
| Reproducibility | DVC for data + git; pinned containers; deterministic seeds |
| CI/CD for ML | GitHub Actions or Azure DevOps + custom gates (eval thresholds) |

## Reproducibility checklist

- [ ] Random seeds fixed (numpy, torch, python `random`)
- [ ] Data versioned (DVC / S3 with hashes)
- [ ] Code at a specific commit
- [ ] Container with pinned dep versions
- [ ] Hardware noted (GPU model, driver, CUDA)

## MLflow tracking (snippet)

```python
import mlflow
import mlflow.sklearn

mlflow.set_experiment("orders-churn")

with mlflow.start_run():
    mlflow.log_params({"n_estimators": 200, "max_depth": 6})
    model.fit(X_train, y_train)
    auc = roc_auc_score(y_test, model.predict_proba(X_test)[:, 1])
    mlflow.log_metric("auc", auc)
    mlflow.sklearn.log_model(model, "model", registered_model_name="orders-churn")
```

## Deploy: ONNX + Triton (or KServe)

```python
# Export
import torch.onnx
torch.onnx.export(model, dummy_input, "model.onnx", opset_version=18,
                  input_names=["input"], output_names=["output"],
                  dynamic_axes={"input": {0: "batch"}})
```

```yaml
# Triton config.pbtxt (minimal)
name: "orders-churn"
backend: "onnxruntime"
max_batch_size: 64
input  [{ name: "input"  data_type: TYPE_FP32 dims: [-1,128] }]
output [{ name: "output" data_type: TYPE_FP32 dims: [-1,2]   }]
```

## Drift signals (monitor in prod)

- Input distribution drift (KS test on numeric, χ² on categorical)
- Prediction drift (output histogram changes)
- Concept drift (label availability lagged: ground truth comes later → eval rolling window)
- Latency / throughput / error rate

## Common Pitfalls

- "Worked on my laptop" → no container, no pipeline, no dataset hash
- Training/serving skew (different preprocessing in each)
- Logging input but not predictions → can't audit
- No retraining trigger → quietly stale model

## See also

- [../MachineLearning](../MachineLearning/) · [../DeepLearning](../DeepLearning/) · [../Evaluation](../Evaluation/) · [../../Observability](../../Observability/)
