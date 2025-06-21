# Change Log

All notable changes to this project will be documented in this file.
This project adheres to [Semantic Versioning](http://semver.org/).

## Version [8.4.1]

- Add version header to UpdateRole method to fix ContentfulException

## Version [8.4.0]

- Added support for Taxonomy endpoints in the Management API
- Added support for Taxonomy concepts in Entry and Asset metadata
- Added new models for TaxonomyConcept and TaxonomyConceptScheme
- Added methods for managing taxonomy concepts and concept schemes

## Version [1.2.0]

- Adds method `CreateEntryForLocaleAsync` to the management client.

## Version [1.1.1]

- Adds missing properties for the `SystemProperties` object.

## Version [1.1.0]

- Adds convenience methods to `ContentfulManagementClient` for retrieveing, creating and updating entries without having to use `Entry<dynamic>`.

## Version [1.0.0]

- Initial stable release on NetStandard 2.0
