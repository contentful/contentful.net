#!/bin/bash
# Run .NET specific api-coverage tests
ENV='"REPO_SDK=dotnet REPO_SLUG=contentful/contentful.net REPO_COMMIT='$CIRCLE_SHA1'"'

BODY="{
  \"request\": {
    \"message\": \"contentful/contentful.net SDK Triggered Request\",
    \"branch\":\"travis_experiments\",
    \"config\": {\"env\": $ENV}
  }
}"

# api-coverage
curl -s -X POST \
	-d "$BODY" \
    -H "Content-Type: application/json" \
    -H "Accept: application/json"   \
    -H "Travis-API-Version: 3"   \
    -H "Authorization: token $TRAVIS_ACCESS_TOKEN" \
    'https://api.travis-ci.com/repo/1336919/requests'
