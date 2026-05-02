# Contract Tests

> Consumer-driven contracts that catch breaking API changes before deployment - without spinning up dependencies.

## Core Concepts

- **Consumer-driven**: the consumer writes a Pact (a JSON contract) describing the requests it makes and the responses it expects.
- **Producer verification**: the producer replays the Pact against its real implementation in CI.
- **Pact Broker**: stores Pacts, links them to versions/environments, powers `can-i-deploy`.
- **HTTP and message pacts**: HTTP pacts for REST APIs; message pacts for Kafka/Service Bus payloads.
- **Not a substitute for E2E** but eliminates 90% of "the upstream team broke us in prod" outages.

## "To Be Dangerous" Cheatsheet

```
Consumer                              Producer
--------                              --------
1. Write expectations                 4. Pull pact from broker
2. Run unit-like test that            5. Replay each interaction against
   generates pact JSON                   real handler, assert it matches
3. Publish pact to broker             6. Publish verification result
                                      7. can-i-deploy --pacticipant ...
```

```bash
# Can-I-Deploy gate (in CI before promote-to-prod):
pact-broker can-i-deploy \
  --pacticipant orders-api \
  --version $GIT_SHA \
  --to-environment production
```

| Library / Tool       | Use                                              |
|----------------------|--------------------------------------------------|
| PactNet              | .NET consumer + provider verification            |
| Pact Broker / Pactflow | Storage, can-i-deploy, webhooks                |
| Schemathesis         | OpenAPI fuzz alternative                         |
| AsyncAPI + Specmatic | Schema-driven contracts for messaging            |

## Quick Reference

- Consumer test produces `pacts/consumer-provider.json`.
- Provider verification: `IPactVerifier` reads the pact and exercises the live API.
- Tag pacts with branch/environment for promotion gates.
- Pact does **not** check semantic correctness, only structural compatibility.

## Common Pitfalls

- Treating Pact as schema validation - it captures only what the consumer actually uses.
- Forgetting provider states - "given an order with id X exists" must be set up by the producer test.
- Pacts drifting because nobody publishes them in CI.
- Strict matching on volatile fields (timestamps, IDs) - use Pact matchers.

## Examples in this folder

- [ConsumerPact.cs](./ConsumerPact.cs) - PactNet consumer test
- [ProviderVerification.cs](./ProviderVerification.cs) - producer side replay

## See also

- [../Integration/README.md](../Integration/README.md)
- [../../Architecture/README.md](../../Architecture/README.md)
