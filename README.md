# Sympli

## Overview

### Task
The CEO at Sympli is very interested in SEO and how it can improve sales. Every morning, he logs into google.com.au, types in the same keywords “e-settlements,” and manually checks where and how many times their company, www.sympli.com.au, appears in the search results. 

To automate this repetitive task, a smart software developer at Sympli decides to write an application that performs this operation and returns the results to the screen. The software receives a string of keywords and a target URL, processes them, and returns a string of numbers indicating the positions where the URL appears in the Google search results. For example, the output could be “1, 10, 33” or “0” (if the URL is not found). 

The CEO is only interested in results where their URL appears within the first 100 search results.

### Extension 1
The CTO at Sympli has added a requirement to limit the number of calls made to Google to one per hour per search. To meet this requirement, caching should be introduced.

### Extension 2
The CEO is impressed with the initial implementation and would like the application to be extended to support and compare results from other search engines, such as Bing. As a developer, you anticipate that additional search engines may need to be supported in the future.

---

## Frontend

- **Technologies**: React, SCSS, TypeScript, Axios, Unit Testing
- **Features**: 
  - A responsive page that allows users to search for results from Google and Bing based on input keywords and target URLs.
  - API integration using Axios.
  - Unit tests implemented with React Testing Library and Jest.
- Refer detail at fe/README.MD

![Frontend Screenshot](https://github.com/user-attachments/assets/95a2107d-fa18-463e-b232-c14c623ee6e7)

---

## Backend

- **Technologies**: Clean Architecture, CQRS, MediatR, Caching, Unit Testing
- **Features**:
  - Modular architecture with a clear separation of concerns.
  - Query and command handling using MediatR.
  - In-memory caching for performance optimization.
  - Unit tests to ensure reliability and maintainability.
- Refer detail at be/README.MD

![Backend Screenshot](https://github.com/user-attachments/assets/846bbc20-4d3e-48b8-a41e-90849fdd0600)
