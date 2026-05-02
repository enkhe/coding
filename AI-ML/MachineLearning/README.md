# Classical ML

> Statistical learning before deep learning. Tabular data + scikit-learn covers more business problems than people think.

## "To Be Dangerous" Cheatsheet

| Task | Algorithm |
|---|---|
| Linear regression | `LinearRegression`, `Ridge`, `Lasso`, `ElasticNet` |
| Classification | `LogisticRegression`, `RandomForestClassifier`, `GradientBoostingClassifier`, **`XGBoost`** / **`LightGBM`** / **`CatBoost`** |
| Multi-class | One-vs-rest / softmax (logreg, GBMs) |
| Clustering | `KMeans`, `DBSCAN`, `HDBSCAN`, hierarchical |
| Dim reduction | `PCA`, `UMAP`, `TSNE` |
| Anomaly | `IsolationForest`, `OneClassSVM` |
| Recommender | Matrix factorization (`Surprise`); ALS for implicit feedback |

## Core workflow

```python
from sklearn.model_selection import train_test_split, cross_val_score
from sklearn.pipeline import Pipeline
from sklearn.preprocessing import StandardScaler
from sklearn.compose import ColumnTransformer
from sklearn.preprocessing import OneHotEncoder
from sklearn.ensemble import GradientBoostingClassifier
from sklearn.metrics import roc_auc_score

X_train, X_test, y_train, y_test = train_test_split(df.drop("y", axis=1), df["y"], test_size=0.2, random_state=1, stratify=df["y"])

pre = ColumnTransformer([
    ("num", StandardScaler(), num_cols),
    ("cat", OneHotEncoder(handle_unknown="ignore"), cat_cols),
])
pipe = Pipeline([("pre", pre), ("clf", GradientBoostingClassifier(random_state=1))])

scores = cross_val_score(pipe, X_train, y_train, cv=5, scoring="roc_auc")
print(scores.mean(), scores.std())

pipe.fit(X_train, y_train)
print("Test AUC:", roc_auc_score(y_test, pipe.predict_proba(X_test)[:, 1]))
```

## Discipline

- **Never tune on test set.** Use CV on train, then a single test eval at the end.
- **Stratified split** for imbalanced labels.
- **Class weights** or **resampling** (SMOTE) for imbalance.
- **Save model + preprocessor as one pipeline** — easier to deploy.

## Common Pitfalls

- Data leakage (info from test in training) → unrealistically good metrics
- Scaling/encoding fit on full data before split — leakage
- "Accuracy" on imbalanced classes → use F1 / AUC / PR curve
- Categorical encoding (target encoding, one-hot) without proper CV folds

## See also

- [../MLNet](../MLNet/) · [../DeepLearning](../DeepLearning/) · [../MLOps](../MLOps/)
