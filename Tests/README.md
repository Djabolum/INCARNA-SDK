# Contract tests

Run the public studio contract harness with:

```powershell
dotnet run --project Tests/ContractHarness/ContractHarness.csproj --configuration Release
```

Expected sentinel:

```text
PASS_STUDIO_CONTRACT_V0_7
```

The harness compiles the public DTOs, morphology guard, and HTTP response
mapper. It verifies safe candidate admission, fail-closed authority and numeric
guards, removal of internal-state and memory fields, and the distinction
between the source projection receipt and the SDK admission decision.
