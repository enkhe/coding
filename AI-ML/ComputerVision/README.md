# Computer Vision

> Images & video. Two eras: classical (OpenCV) and deep learning (CNNs, ViTs, multimodal LLMs).

## Tasks map

| Task | Tool |
|---|---|
| Classification | torchvision pretrained (ResNet, ViT) |
| Detection | YOLO (Ultralytics), DETR |
| Segmentation | SAM2 (Segment Anything), Mask R-CNN |
| OCR | Tesseract (classical), TrOCR, **Azure Document Intelligence**, GPT-5/Claude vision |
| Embedding | CLIP, OpenCLIP, SigLIP |
| Multimodal Q&A | GPT-5 vision, Claude vision |

## Quick Reference (torchvision pretrained)

```python
import torch
from torchvision import models, transforms
from PIL import Image

device = "cuda" if torch.cuda.is_available() else "cpu"
weights = models.ResNet50_Weights.IMAGENET1K_V2
model = models.resnet50(weights=weights).to(device).eval()
preprocess = weights.transforms()

img = Image.open("cat.jpg").convert("RGB")
x = preprocess(img).unsqueeze(0).to(device)

with torch.no_grad():
    logits = model(x)
top5 = logits.softmax(dim=1).topk(5)
for p, idx in zip(top5.values[0], top5.indices[0]):
    print(weights.meta["categories"][idx], float(p))
```

## CLIP — text+image embeddings (semantic image search)

```python
import open_clip
model, _, preprocess = open_clip.create_model_and_transforms("ViT-B-32", pretrained="laion2b_s34b_b79k")
tokenizer = open_clip.get_tokenizer("ViT-B-32")

img_emb = model.encode_image(preprocess(img).unsqueeze(0))           # 512-dim
txt_emb = model.encode_text(tokenizer(["a photo of a cat"]))         # 512-dim
similarity = (img_emb @ txt_emb.T).softmax(dim=-1)
```

## Multimodal LLMs (the modern path for many tasks)

```python
# Anthropic SDK example — Claude vision
from anthropic import Anthropic
client = Anthropic()
img_b64 = base64.b64encode(open("invoice.png", "rb").read()).decode()

resp = client.messages.create(
    model="claude-opus-4-7",
    max_tokens=1024,
    messages=[{
        "role": "user",
        "content": [
            {"type": "image", "source": {"type": "base64", "media_type": "image/png", "data": img_b64}},
            {"type": "text", "text": "Extract vendor, total, and date as JSON."}
        ]
    }]
)
print(resp.content[0].text)
```

## See also

- [../DeepLearning](../DeepLearning/) · [../LLMs](../LLMs/) · [../VectorSearch](../VectorSearch/)
