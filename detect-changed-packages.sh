#!/bin/bash

# Usage:
#   ./scripts/detect-changed-packages.sh <PROJECT_NAME>
# Outputs:
#   "changed=true" or "changed=false"

set -e

PROJECT=$1

git fetch origin main

if [ "$PROJECT" = "DepenMock" ]; then
  TARGET_PATH="src/DepenMock"
else
  TARGET_PATH="src/DepenMock src/$PROJECT"
fi

echo "Checking for changes in: $TARGET_PATH"

if git diff --quiet origin/main -- $TARGET_PATH; then
  echo "changed=false"
else
  echo "changed=true"
fi
