#!/usr/bin/env bash
git push --follow-tags origin main
dotnet pack
dotnet nuget push