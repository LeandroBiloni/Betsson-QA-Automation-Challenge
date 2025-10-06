# qa-backend-code-challenge

Code challenge for QA Backend Engineer candidates.

### Build Docker image

Run this command from the directory where there is the solution file.

```
docker build -f src/Betsson.OnlineWallets.Web/Dockerfile .
```

### Run Docker container

```
docker run -p <port>:8080 <image id>
```

### Open Swagger

```
http://localhost:<port>/swagger/index.html
```

### Run Tests

Run this command in the root directory to run all tests. Run in a specific test project to run the tests in that project.

```
dotnet test
```

### Run Tests with logs

Run this command in the root directory to run all tests. Run in a specific test project to run the tests in that project.

```
dotnet test --logger "console;verbosity=detailed"
```