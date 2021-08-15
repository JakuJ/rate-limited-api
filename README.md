# rate-limiting

Requirements:

- dotnet CLI

### TODO:

- SSL certificate verification?
- Docker support?

### Objective

Since this is a recruitment task, the app shall be as minimal as possible, while fulfilling all requirements.

This means:

- no persistence – I use EFCore, but with an in-memory provider
- no Redis - rate limits for each user cached in-memory within the same process.

### Assumptions:

- Users have an unique ID assigned to them
    - Since we are using BasicAuth, the user has a username and a password
    - We enforce some arbitrary length requirements for the username (5 - 60 characters) and the password (8 - 60
      characters)
    - Stored passwords are hashed and salted with Argon2
    - We expose a POST endpoint (/register) for registering new users
    - Prohibiting users from registering new accounts every millisecond is out of scope of this task