#!/bin/sh
set -eax

#Exec the CarboneAwareCLI, to return the Region with Lowest Emissions
# $1 : input.location
# $2: input.config
# $3 : --lowest
OutputEmissionsData=$(/caw -l $1 $2 ) 

#Export the Recommended Region, as a Github Action output, to be used by subsequent workflow steps
# the CLI might return several Regions, for the current version of the Github Action, we return one of the lowest
echo "LowestEmissionsLocation=$(echo $OutputEmissionsData | jq '.[0].Location')" >> $GITHUB_OUTPUT
