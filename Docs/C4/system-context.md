# C4 Level 1 — System Context: Order Management System

> One box for the system, plus the people and external systems that interact with it.

## Diagram

```mermaid
C4Context
    title System Context — Order Management System (OMS)

    Person(customer, "Customer", "Places and tracks orders online.")
    Person(warehouse, "Warehouse Operator", "Picks, packs, and ships orders.")

    System(oms, "Order Management System", "Lets customers place orders and warehouse staff fulfill them.")

    System_Ext(payments, "Payments Gateway", "Processes credit-card charges (Stripe).")
    System_Ext(shipping, "Shipping Carriers", "Calculates rates and creates shipping labels.")
    System_Ext(email, "Email / SMS Provider", "Sends order confirmations and shipment updates (SendGrid).")

    Rel(customer, oms, "Browses, places, and tracks orders", "HTTPS")
    Rel(warehouse, oms, "Manages fulfillment", "HTTPS")
    Rel(oms, payments, "Authorises and captures payments", "HTTPS / REST")
    Rel(oms, shipping, "Requests rates and creates labels", "HTTPS / REST")
    Rel(oms, email, "Sends transactional notifications", "HTTPS / REST")
```

## Reading the diagram

- **One system, two user types, three external systems.** Anything outside the OMS box is "not our problem to build" — but it is our problem to integrate with.
- This is the diagram you put in front of a non-technical stakeholder. No technology choices appear yet.

## See also

- [container-diagram.md](./container-diagram.md) — zoom into the OMS box.
- [README.md](./README.md) — C4 model overview.
