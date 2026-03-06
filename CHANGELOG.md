# Change Log

## Master
- Aligned Taxonomy API

## Version [8.6.0]
- Support querying content based on tags

## Version [8.5.0]
- Support locale based publishing

## Version [8.4.3]
- Allow DirectApiUrl to be configurable 

## Version [8.4.2]
- Remove extra test methods
- Fix SystemProperties serialization to ignore null values

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
