# Deep Learning

> Neural networks. PyTorch is the default research/production framework in 2026.

## Mental model

- **Model** = stack of layers (linear, conv, attention, normalization, activation)
- **Loss** = how wrong (cross-entropy for classification, MSE for regression, contrastive, etc.)
- **Optimizer** = how to step weights (Adam, AdamW, Lion, SGD)
- **Backprop** = autograd computes gradients; you call `loss.backward()` and `optimizer.step()`

## "To Be Dangerous" — PyTorch loop

```python
import torch
from torch import nn, optim
from torch.utils.data import DataLoader

device = "cuda" if torch.cuda.is_available() else "cpu"

class MLP(nn.Module):
    def __init__(self, in_dim, n_classes):
        super().__init__()
        self.net = nn.Sequential(
            nn.Linear(in_dim, 256), nn.ReLU(),
            nn.Dropout(0.2),
            nn.Linear(256, n_classes),
        )
    def forward(self, x): return self.net(x)

model = MLP(in_dim=128, n_classes=10).to(device)
opt = optim.AdamW(model.parameters(), lr=3e-4, weight_decay=0.01)
loss_fn = nn.CrossEntropyLoss()

for epoch in range(10):
    model.train()
    for x, y in train_loader:
        x, y = x.to(device), y.to(device)
        opt.zero_grad()
        logits = model(x)
        loss = loss_fn(logits, y)
        loss.backward()
        opt.step()

    model.eval()
    correct = 0; total = 0
    with torch.no_grad():
        for x, y in val_loader:
            x, y = x.to(device), y.to(device)
            preds = model(x).argmax(dim=-1)
            correct += (preds == y).sum().item()
            total += y.size(0)
    print(f"epoch {epoch} val_acc {correct/total:.4f}")

torch.save(model.state_dict(), "model.pt")
```

## Useful libraries

| Need | Library |
|---|---|
| Vision | `torchvision` (transforms, pretrained) |
| Text | `transformers` (Hugging Face) |
| Training loops | `pytorch-lightning`, `accelerate` |
| ONNX export | `torch.onnx.export` |
| Quantization | `torch.ao.quantization`, `bitsandbytes` |
| LoRA / PEFT | `peft` |
| Distributed | `torch.distributed`, `accelerate`, `DeepSpeed` |

## Common Pitfalls

- Forgetting `model.train()` / `model.eval()` — affects BatchNorm/Dropout
- Forgetting `optimizer.zero_grad()` — gradients accumulate across batches
- `.cpu()` → `.numpy()` accidentally during training (loses graph)
- OOM from too-large batches — use gradient accumulation or smaller batch
- Comparing `nn.Module` outputs directly without `.detach()` for logging

## See also

- [../MachineLearning](../MachineLearning/) · [../NLP](../NLP/) · [../ComputerVision](../ComputerVision/) · [../MLOps](../MLOps/)
