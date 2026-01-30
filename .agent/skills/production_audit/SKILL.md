---
name: production-code-audit
description: "Autonomously deep-scan entire codebase line-by-line, understand architecture and patterns, then systematically transform it to production-grade, corporate-level professional quality with optimizations"
---

# Production Code Audit

## Overview

Autonomously analyze the entire codebase to understand its architecture, patterns, and purpose, then systematically transform it into production-grade, corporate-level professional code. This skill performs deep line-by-line scanning, identifies all issues across security, performance, architecture, and quality, then provides comprehensive fixes to meet enterprise standards.

## When to Use This Skill

- Use when user says "make this production-ready"
- Use when user says "audit my codebase"
- Use when user says "make this professional/corporate-level"
- Use when user says "optimize everything"
- Use when user wants enterprise-grade quality
- Use when preparing for production deployment
- Use when code needs to meet corporate standards

## How It Works

### Step 1: Autonomous Codebase Discovery

**Automatically scan and understand the entire codebase:**

1. **Read all files** - Scan every file in the project recursively
2. **Identify tech stack** - Detect languages, frameworks, databases, tools
3. **Understand architecture** - Map out structure, patterns, dependencies
4. **Identify purpose** - Understand what the application does
5. **Find entry points** - Locate main files, routes, controllers
6. **Map data flow** - Understand how data moves through the system

**Do this automatically without asking the user.**

### Step 2: Comprehensive Issue Detection

**Scan line-by-line for all issues:**

**Architecture Issues:**

- Circular dependencies
- Tight coupling
- God classes (>500 lines or >20 methods)
- Missing separation of concerns
- Poor module boundaries
- Violation of design patterns

**Security Vulnerabilities:**

- SQL injection (string concatenation in queries)
- XSS vulnerabilities (unescaped output)
- Hardcoded secrets (API keys, passwords in code)
- Missing authentication/authorization
- Weak password hashing (MD5, SHA1)
- Missing input validation
- CSRF vulnerabilities
- Insecure dependencies

**Performance Problems:**

- N+1 query problems
- Missing database indexes
- Synchronous operations that should be async
- Missing caching
- Inefficient algorithms (O(n²) or worse)
- Large bundle sizes
- Unoptimized images
- Memory leaks

**Code Quality Issues:**

- High cyclomatic complexity (>10)
- Code duplication
- Magic numbers
- Poor naming conventions
- Missing error handling
- Inconsistent formatting
- Dead code
- TODO/FIXME comments

**Testing Gaps:**

- Missing tests for critical paths
- Low test coverage (<80%)
- No edge case testing
- Flaky tests
- Missing integration tests

**Production Readiness:**

- Missing environment variables
- No logging/monitoring
- No error tracking
- Missing health checks
- Incomplete documentation
- No CI/CD pipeline

### Step 3: Automatic Fixes and Optimizations

**Fix everything automatically:**

1. **Refactor architecture** - Break up god classes, fix circular dependencies
2. **Fix security issues** - Use parameterized queries, remove secrets, add validation
3. **Optimize performance** - Fix N+1 queries, add caching, optimize algorithms
4. **Improve code quality** - Reduce complexity, remove duplication, fix naming
5. **Add missing tests** - Write tests for untested critical paths
6. **Add production infrastructure** - Logging, monitoring, health checks
7. **Optimize everything** - Bundle size, images, database queries
8. **Add documentation** - README, API docs, architecture docs

### Step 4: Verify and Report

**After making all changes:**

1. Run all tests to ensure nothing broke
2. Verify all security issues are fixed
3. Measure performance improvements
4. Generate comprehensive report
5. Provide before/after metrics

## Best Practices

### ✅ Do This

- **Scan Everything** - Read all files, understand entire codebase
- **Fix Automatically** - Don't just report, actually fix issues
- **Prioritize Critical** - Security and data loss issues first
- **Measure Impact** - Show before/after metrics
- **Verify Changes** - Run tests after making changes
- **Be Comprehensive** - Cover architecture, security, performance, testing
- **Optimize Everything** - Bundle size, queries, algorithms, images
- **Add Infrastructure** - Logging, monitoring, error tracking
- **Document Changes** - Explain what was fixed and why

### ❌ Don't Do This

- **Don't Ask Questions** - Understand the codebase autonomously
- **Don't Wait for Instructions** - Scan and fix automatically
- **Don't Report Only** - Actually make the fixes
- **Don't Skip Files** - Scan every file in the project
- **Don't Ignore Context** - Understand what the code does
- **Don't Break Things** - Verify tests pass after changes
- **Don't Be Partial** - Fix all issues, not just some

## Production Audit Checklist

### Security

- [ ] No SQL injection vulnerabilities
- [ ] No hardcoded secrets
- [ ] Authentication on protected routes
- [ ] Authorization checks implemented
- [ ] Input validation on all endpoints
- [ ] Password hashing with bcrypt (10+ rounds)
- [ ] HTTPS enforced
- [ ] Dependencies have no vulnerabilities

### Performance

- [ ] No N+1 query problems
- [ ] Database indexes on foreign keys
- [ ] Caching implemented
- [ ] API response time < 200ms
- [ ] Bundle size < 200KB (gzipped)

### Testing

- [ ] Test coverage > 80%
- [ ] Critical paths tested
- [ ] Edge cases covered
- [ ] No flaky tests
- [ ] Tests run in CI/CD

### Production Readiness

- [ ] Environment variables configured
- [ ] Error tracking setup (Sentry)
- [ ] Structured logging implemented
- [ ] Health check endpoints
- [ ] Monitoring and alerting
- [ ] Documentation complete
