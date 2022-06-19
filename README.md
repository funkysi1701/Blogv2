# Metrics

Repo for metrics.funkysi1701.com

```mermaid
graph TD
    A[Azure Static Web App]---B{Http Fn}
    B---D[(Database)]
    C{Timer Fn}---D
    D---E[Import Console App]
    F([App Insights])---A
    F---B
    F---C
```
