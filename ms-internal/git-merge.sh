#!/bin/bash

BRANCH=${1:-"auto-pr-$RANDOM"}
BASE=${2:-"dev"}
UPSTREAM=${3:-"Green-Software-Foundation/carbon-aware-sdk"}

UPSTREAMREPO="https://github.com/$UPSTREAM"}
COMMITMESSAGE="Warning: git push of upstream contents failed, saving error info into an empty-PR for investigation"
STATUS=0

git config user.name "GitHub Actions Bot"
git config user.email "<>"

# upstream contents needed for creating the PR
git remote add upstream $UPSTREAMREPO
git fetch upstream
git checkout -b $BRANCH upstream/$BASE 

# git push, capturing output text and exit code
echo "Creating a PR with fetched-contents from upstream:$BASE into origin:$BASE."
GIT_PUSH_OUTPUT=$(git push --set-upstream origin $BRANCH 2>&1) # without 'workflow' permission this fails if changes to .github\workflow\*.yml file(s) are present
STATUS=$?
echo $GIT_PUSH_OUTPUT

# 'Failure-to-Launch' if git push fails -- create and empty-PR with the error details for manual resolution
if [ $STATUS -ne 0 ]; then
    echo $COMMITMESSAGE

    # 1. re-create branch from origin:dev
    git checkout -f $BASE
    git branch -D $BRANCH
    git fetch origin
    git checkout -b $BRANCH origin/$BASE

    # 2. add + commit the error details
    echo $COMMITMESSAGE > README-$BRANCH.md
    echo "-------------" >> README-$BRANCH.md
    echo $GIT_PUSH_OUTPUT >> README-$BRANCH.md
    git add README-$BRANCH.md
    git commit -m "$COMMITMESSAGE"

    # 3. push to origin
    git push --set-upstream origin $BRANCH         
    STATUS=$?
fi

# tell next pipeline step if push failed
exit $STATUS
