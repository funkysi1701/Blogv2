# Metrics

Repo for metrics.funkysi1701.com

```mermaid
graph LR
    B{Http Fn}--->A[Azure Static Web App]
    D[(Database)]--->B
    C{Timer Fn}--->D
    E[Import Console App]--->D
    A--->F([App Insights])
    B--->F
    C--->F
```
Archive please see https://github.com/funkysi1701/Metrics
