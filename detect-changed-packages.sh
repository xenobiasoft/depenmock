#!/bin/bash
# Compares package folders to determine which have changed

PACKAGES=("DepenMock" "DepenMock.XUnit" "DepenMock.NUnit" "DepenMock.MSTest")
BASE_BRANCH=${1:-origin/main}

echo "Comparing to $BASE_BRANCH..."
echo ""
for pkg in "${PACKAGES[@]}"; do
  if git diff --quiet $BASE_BRANCH -- "$pkg"; then
    echo "❌ $pkg – no changes"
  else
    echo "✅ $pkg – changed"
  fi
done
