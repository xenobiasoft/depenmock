#!/bin/bash

# Usage:
#   ./detect-changed-packages.sh <PROJECT_NAME>
# Outputs:
#   "changed=true" or "changed=false"

set -e

PROJECT=$1

if [ -z "$PROJECT" ]; then
  echo "Error: Missing PROJECT argument."
  echo "Usage: ./detect-changes.sh <PROJECT_NAME>"
  exit 1
fi
git fetch origin main

if [ "$PROJECT" = "DepenMock" ]; then
  TARGET_PATH="DepenMock"
else
  TARGET_PATH="DepenMock $PROJECT"
fi

echo "Checking for changes in: $TARGET_PATH"

if git diff --quiet origin/main -- $TARGET_PATH; then
  echo "changed=true" # temp change to force a publish/release
else
  echo "changed=true"
fi
