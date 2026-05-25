# Testing Guides

This folder contains all developer-facing documentation related to **testing strategy, patterns, and practices** across the CampFitFurDogs backend and frontend.

These guides explain *how to test*, not *what to test*.  
They complement the architectural guides by showing how tests map to the system’s layers.

---

# Contents

- **[Test Architecture](../test-architecture.md)**  
  How tests map to API, Application, and Domain layers.

- **[Integration Testing](../integration-testing.md)**  
  How to write integration tests using the test host, fake services, and the dispatcher.

- **[Frontend Testing](../frontend-testing.md)**  
  How to test React components, forms, and UI flows.

- **[Form Testing](../form-testing.md)**  
  How to test form validation, submission, and error states.

---

# Purpose

These guides help developers:

- Write tests at the correct layer  
- Avoid over-testing or under-testing  
- Use the test host and fakes correctly  
- Understand how validation boundaries affect test placement  
- Maintain consistent testing patterns across vertical slices  

---

# Related Documentation

- [Validation Boundaries](ca://s?q=Show_validation_boundaries_doc)  
- [Dispatcher Pipeline](ca://s?q=Show_dispatcher_pipeline_guide)  
- [API Endpoint Purity](ca://s?q=Show_API_endpoint_purity_guide)
