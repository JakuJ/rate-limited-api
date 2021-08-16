# rate-limiting

Requirements:

- dotnet CLI

### Objective

Since this is a recruitment task, the app shall be as minimal as possible, while fulfilling all requirements.

This means:

- no persistence – I use EFCore, but with an in-memory provider
- no Redis - rate limits for each user stored in-memory within the server process.

### Assumptions:

- Users have an unique ID assigned to them
    - Since we are using BasicAuth, the user has a username and a password
    - Requirements: username is [6 - 64] characters, password is [8 - 64]
      characters, both cannot contain a ':' symbol
    - Stored passwords are hashed and salted with Argon2
    - We expose a POST endpoint (/register) for registering new users
    - Prohibiting users from registering new accounts every millisecond is out of scope of this task