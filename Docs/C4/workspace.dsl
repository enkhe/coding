// Structurizr DSL — Order Management System
// One source of truth for Context, Container, and Component views.
// Render with the Structurizr CLI, Structurizr Lite, or any DSL-compatible viewer.

workspace "OMS" "Order Management System reference architecture" {

    model {
        customer  = person "Customer" "Places and tracks orders."
        warehouse = person "Warehouse Operator" "Picks, packs, and ships orders."

        payments  = softwareSystem "Payments Gateway" "Stripe" "External"
        shipping  = softwareSystem "Shipping Carriers" "Carrier APIs" "External"
        email     = softwareSystem "Email / SMS Provider" "SendGrid" "External"

        oms = softwareSystem "Order Management System" "Lets customers order and warehouse fulfill." {
            spa     = container "Customer SPA"      "Storefront UI"               "Blazor WebAssembly"
            portal  = container "Warehouse Portal"  "Internal fulfillment UI"     "Blazor Server"
            api     = container "Order API"         "Public + internal HTTP API"  "ASP.NET Core 10" {
                endpoints   = component "HTTP Endpoints"     "Minimal APIs + OpenAPI"
                catalog     = component "Catalog Module"     "Products, prices"
                orders      = component "Orders Module"      "Cart, orders, history"
                paymentsMod = component "Payments Module"    "Wraps Stripe"
                outbox      = component "Outbox Dispatcher"  "Reliable event publish"
                telemetry   = component "Telemetry"          "OpenTelemetry"
            }
            worker  = container "Fulfillment Worker" "Async event handlers"        ".NET 10 Worker"
            db      = container "Orders DB"          "Authoritative data"          "SQL Server 2025" "Database"
            cache   = container "Cache"              "Catalog + idempotency keys"  "Redis"           "Database"
            bus     = container "Message Broker"     "Order events, outbox"        "Azure Service Bus" "Queue"
        }

        // Context-level relationships
        customer  -> oms      "Browses and orders"
        warehouse -> oms      "Manages fulfillment"
        oms       -> payments "Charges cards"
        oms       -> shipping "Creates labels"
        oms       -> email    "Sends notifications"

        // Container-level relationships
        customer  -> spa      "Uses"      "HTTPS"
        warehouse -> portal   "Uses"      "HTTPS"
        spa       -> api      "Calls"     "HTTPS / JSON"
        portal    -> api      "Calls"     "HTTPS / JSON"
        api       -> db       "Reads/writes" "EF Core 10"
        api       -> cache    "Reads/writes" "Redis"
        api       -> bus      "Publishes events" "AMQP"
        api       -> payments "Authorises payments" "HTTPS"
        bus       -> worker   "Delivers events" "AMQP"
        worker    -> shipping "Creates labels" "HTTPS"
        worker    -> email    "Sends notifications" "HTTPS"
        worker    -> db       "Updates fulfillment" "EF Core 10"

        // Component-level relationships
        spa         -> endpoints   "Calls" "HTTPS / JSON"
        endpoints   -> catalog
        endpoints   -> orders
        orders      -> paymentsMod
        paymentsMod -> payments    "Charges card" "HTTPS"
        orders      -> db          "Persists" "EF Core 10"
        catalog     -> db          "Reads" "EF Core 10"
        orders      -> outbox      "Writes events"
        outbox      -> bus         "Publishes" "AMQP"
        endpoints   -> telemetry   "Emits"
        orders      -> telemetry   "Emits"
    }

    views {
        systemContext oms "Context" {
            include *
            autolayout lr
        }
        container oms "Containers" {
            include *
            autolayout lr
        }
        component api "OrderApiComponents" {
            include *
            autolayout lr
        }

        styles {
            element "Person"   { shape Person   background #08427b color #ffffff }
            element "External" { background #999999 color #ffffff }
            element "Database" { shape Cylinder }
            element "Queue"    { shape Pipe }
        }
    }
}
