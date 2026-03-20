# Contributing

Thanks for helping improve `contentful.net`.

## Development with Dev Containers

This repository includes a `.devcontainer` configuration for a reproducible local setup. GitHub Actions uses the same devcontainer configuration for CI.

1. Install Docker and a devcontainer-compatible editor. Visual Studio Code with the Dev Containers extension works well.
2. Open the repository in the dev container and wait for the post-create setup to finish.
3. Verify the environment:

```bash
dotnet build && dotnet test Contentful.AspNetCore.Tests && dotnet test Contentful.Core.Tests
```

## Other Useful Commands

```bash
dotnet restore
dotnet test Contentful.AspNetCore.Tests
dotnet test Contentful.Core.Tests
```

## Pull Requests

1. Fork the repository and create a branch for your change.
2. Run the relevant checks from the dev container.
3. Open a pull request with a short summary of the change and any follow-up context.
