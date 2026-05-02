# Django

> Batteries-included Python web framework. ORM, admin, auth, migrations, templating. Pair with **Django REST Framework** for APIs.

## Anatomy

```
project/
├── manage.py
├── project/
│   ├── settings.py
│   ├── urls.py
│   ├── asgi.py / wsgi.py
└── orders/                 # an "app"
    ├── models.py
    ├── views.py
    ├── urls.py
    ├── admin.py
    ├── serializers.py      # DRF
    └── migrations/
```

## Quick Reference (DRF)

```python
# orders/models.py
from django.db import models
import uuid

class Order(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4)
    user_id = models.UUIDField()
    amount = models.DecimalField(max_digits=18, decimal_places=2)
    placed_at = models.DateTimeField(auto_now_add=True)

# orders/serializers.py
from rest_framework import serializers
class OrderSerializer(serializers.ModelSerializer):
    class Meta: model = Order; fields = "__all__"

# orders/views.py
from rest_framework import viewsets
from rest_framework.permissions import IsAuthenticated
class OrderViewSet(viewsets.ModelViewSet):
    queryset = Order.objects.all()
    serializer_class = OrderSerializer
    permission_classes = [IsAuthenticated]

# orders/urls.py
from rest_framework.routers import DefaultRouter
router = DefaultRouter()
router.register(r"orders", OrderViewSet)
urlpatterns = router.urls
```

## "To Be Dangerous"

| Task | Command |
|---|---|
| Create app | `django-admin startapp orders` |
| Migrations | `python manage.py makemigrations` / `migrate` |
| Admin user | `python manage.py createsuperuser` |
| Async views | `async def view(request):` (Django 4.1+) |
| Caching | `django.core.cache` (Redis backend) |
| Channels | Django Channels for WebSockets |

## Common Pitfalls

- N+1 queries — use `.select_related()` / `.prefetch_related()`
- DEBUG=True in prod → leaks
- Default DB SQLite in prod → switch to Postgres
- Admin panel exposed publicly → restrict by URL or IP

## See also

- [../FastAPI](../FastAPI/) · [../Flask](../Flask/)
