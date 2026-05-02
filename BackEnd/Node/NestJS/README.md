# NestJS

> Angular-flavored Node framework. Decorators, DI, modules. The closest thing to ASP.NET Core in Node-land.

## Concepts

- **Module** — DI scope; declares providers/controllers/imports/exports
- **Controller** — HTTP endpoints
- **Service / Provider** — injectable business logic
- **DTO** — request/response shapes (`class-validator` + `class-transformer`)
- **Guard / Interceptor / Pipe** — cross-cutting (auth, transformation, validation)

## Quick Reference

```ts
// orders.module.ts
@Module({
  imports: [TypeOrmModule.forFeature([Order])],
  controllers: [OrdersController],
  providers: [OrdersService],
})
export class OrdersModule {}

// place-order.dto.ts
export class PlaceOrderDto {
  @IsUUID() userId!: string;
  @IsNumber() @Min(0.01) amount!: number;
}

// orders.controller.ts
@Controller('orders')
@UseGuards(AuthGuard)
export class OrdersController {
  constructor(private svc: OrdersService) {}

  @Post()
  @HttpCode(201)
  place(@Body() dto: PlaceOrderDto) {
    return this.svc.place(dto);
  }

  @Get(':id')
  async get(@Param('id') id: string) {
    const order = await this.svc.find(id);
    if (!order) throw new NotFoundException();
    return order;
  }
}

// main.ts
const app = await NestFactory.create(AppModule);
app.useGlobalPipes(new ValidationPipe({ whitelist: true, transform: true }));
await app.listen(8080);
```

## Common Pitfalls

- Forgetting `ValidationPipe` globally → DTOs not validated
- Service/repo cyclic imports — break with interfaces
- Anti-pattern: putting business logic in controllers
- Provider scope (`Scope.REQUEST`) defaults to `DEFAULT` (singleton) — surprises with stateful services

## See also

- [../Express](../Express/) · [../Fastify](../Fastify/) · [../../CSharp/AspNetCore](../../CSharp/AspNetCore/)
