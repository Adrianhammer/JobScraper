# Lambda CI/CD Options (JobScraper)

This doc outlines the two common ways to update your Lambda in the future.

## Option A: Manual upload (simplest while learning)

Use this when you want full control and minimal AWS setup.

1) Package the Lambda zip:

```bash
./scripts/lambda-package.sh
```

2) Upload the zip in the AWS Console:
- Lambda → Code → Upload from → `.zip file`
- Pick: `bin/Release/net10.0/JobScraper.zip`

Pros:
- Simple and transparent
- No AWS credentials in GitHub

Cons:
- Manual steps every change

## Option B: GitHub Actions deploy (best practice)

Use this when you want every push to `main` to update Lambda automatically.

High-level flow:
1) GitHub Actions builds the project
2) Packages the Lambda zip
3) Deploys to the Lambda function

You will need:
- An AWS IAM user with permissions to update the Lambda
- GitHub Secrets for:
  - `AWS_ACCESS_KEY_ID`
  - `AWS_SECRET_ACCESS_KEY`
  - `AWS_REGION`
  - `LAMBDA_FUNCTION_NAME`

You can then create a workflow like:
- `.github/workflows/deploy-lambda.yml`
- Trigger: on push to `main`
- Steps:
  - checkout
  - setup .NET 10
  - `dotnet lambda package`
  - `aws lambda update-function-code`

Pros:
- One command (git push)
- Standard best practice

Cons:
- Requires AWS IAM + GitHub secrets
- Slightly more setup

## Recommendation

Start with **Option A** until you’re comfortable, then move to **Option B**.
When you’re ready, ask for a CI workflow and we can generate the exact YAML.
