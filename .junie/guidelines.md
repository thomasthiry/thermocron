# Development Guidelines for Junie

## Git Commit Guidelines
- **Always use single-line commit messages maximum**
- Keep commits concise and descriptive
- No multi-line commit messages or detailed descriptions
- Always execute tests before considering committing, if tests fail, do not propose to commit
- Always propose to commit after running the tests and they passed

## Development Methodology - Test Driven Development (TDD)
**STRICT TDD WORKFLOW:**
1. **I will ask you to create a test** - Write ONLY the test, NO implementation
2. **Run the test** - It MUST fail (red phase)
3. **I will ask you to write implementation** - Write MINIMAL code to make test pass
4. **Never do more than asked** - Only implement what the test specifies
5. **Simplest code that works** - No extra features, no future-proofing, just enough