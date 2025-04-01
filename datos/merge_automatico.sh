#!/bin/bash

# Lista los PRs abiertos en formato JSON
cd ../tp || { echo "No se pudo cambiar al directorio ../tp"; exit 1; }

gh pr list --state open --json number,mergeable | \
  jq -c '.[]' | while read -r pr; do
    number=$(echo "$pr" | jq -r '.number')
    mergeable=$(echo "$pr" | jq -r '.mergeable')

    if [[ "$mergeable" == "MERGEABLE" ]]; then
      echo "🔀 Haciendo merge del PR #$number..."
      gh pr merge "$number" --merge --delete-branch --admin
    else
      echo "⚠️  PR #$number no es mergeable: $mergeable"
    fi
done