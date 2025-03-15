# MyThings.TaskManagementSystem

---

# System Design:

CDN: Ensures low-latency global content delivery by caching static assets (HTML/CSS/JS) at edge locations.

Load Balancer: Enables horizontal scaling and prevents server overload by evenly routing traffic.

API Gateway: Enforces rate limiting to protect backend services from abuse/overload.

NoSQL DB: Handles write-heavy workloads at scale with horizontal partitioning and fits simple key-value data (email + timestamp) (DynamoDB).

Message Queue: Decouples processes for reliability, ensuring email delivery retries without blocking subscriptions (SQS / Kafka ).

Monitoring: Provides real-time visibility into system health (API latency, queue depth) for proactive troubleshooting (Grafana).
