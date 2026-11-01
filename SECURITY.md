# Security and local configuration

This is a public example repository. **Never commit passwords, connection strings containing credentials, API tokens, private keys, or real customer data.**

The tracked `src/Life.Api/appsettings.json` file must not contain a database password. For local development, supply the complete connection string through the environment variable `ConnectionStrings__DefaultConnection` (or an appropriately excluded local configuration file), and keep sensitive values out of screenshots, logs, issues, and pull requests.

If a credential ever appears in the repository, **rotate/revoke it immediately** before removing it from tracked files. Removing it in a later commit does not eliminate it from older commits or previously cloned copies. Consider repository-history remediation separately after auditing references, forks, deployments, and consumer clones.

For possible vulnerabilities or disclosures, contact the maintainer privately where possible rather than posting the secret in a public issue.
