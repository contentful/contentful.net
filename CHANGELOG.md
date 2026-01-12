# Change Log

## Unreleased/master

## Version [8.4.3]
- Allow DirectApiUrl to be configurable

## Version [8.4.2]
- Remove extra test methods
- Fix SystemProperties serialization to ignore null values

## Version [8.4.1]
- Add version header to UpdateRole method to fix ContentfulException

## Version [8.4.0]
- Added support for Taxonomy endpoints in the Management API

## Version [8.3.2]
- Fix: store original request message to prevent ObjectDisposedException during rate limit retries

## Version [8.3.1]
- Fix: update CircleCI config and fix syntax error to make build pass

## Version [8.3.0]
- Fix edge case for resolving embedded entries without a strong type

## Version [8.2.1]
- Add option for management client base url

## Version [8.2.0]
- Contentful client tweaks

## Version [8.1.0]
- Feature/fix resolving again

## Version [8.0.1]
- Add missing renderer
- Update reference resolve
- Fix: impl suggest recursive fix
- Add support for cross space references without having settings
- Update IFieldValidation.cs enum EnabledMarkRestrictions with 'strikethrough'
- Add base url to public api

## Version [8.0.0]
- Major version update

## Version [7.5.0]
- Add serializer to calls for management entries and fix minor perf issue

## Version [7.4.3]
- Fix capitalization typo

## Version [7.4.2]
- Add resolution for contentful urn entries

## Version [7.4.1]
- Make nullvalue handling ignore for ContentTypeField

## Version [7.4.0]
- Add support for x-space refs

## Version [7.3.0]
- Add support for tags to delivery client

## Version [7.2.12]
- Add public to minHeight

## Version [7.2.11]
- Add support for slug editor interface control settings

## Version [7.2.10]
- Add json converter for layoutgroups

## Version [7.2.9]
- Update naming for compose properties

## Version [7.2.8]
- Update serialization of update and create for content types

## Version [7.2.7]
- Add support for editorinterface group settings

## Version [7.2.6]
- Add support for content type metadata

## Version [7.2.5]
- Fix serialization of datepicker format

## Version [7.2.4]
- Correctly deserialize sub- and superscript enabled marks

## Version [7.2.3]
- Add support for sub and superscript to html-renderer and update versions for release

## Version [7.2.2]
- Fix quality for imagetaghelper and add raw endpoint

## Version [7.2.0]
- Add support for cross space references
- Pass the MaxDepth from the JsonSetting to the JsonConverter

## Version [7.1.0]
- Add support for e-tags

## Version [7.0.1]
- Update package version and dependency
- Remove unused dependencies

## Version [7.0.0]
- Rename misspelled method name

## Version [6.0.18]
- Add support for default-values

## Version [6.0.16]
- Update package version and dependency

## Version [6.0.15]
- Update package version and dependency

## Version [6.0.14]
- Update package version and dependency

## Version [6.0.13]
- Fix asset rendering with null file

## Version [6.0.12]
- Fix some encoding issues and add support for tables

## Version [6.0.11]
- Update package version and dependency

## Version [6.0.10]
- Add content collection to horizontal ruler

## Version [6.0.9]
- Update package version and dependency

## Version [6.0.8]
- Add test and fix for * localized content

## Version [6.0.7]
- Add overload to webhook consumer

## Version [6.0.6]
- Add visibility for tags

## Version [6.0.5]
- Add async consumer

## Version [6.0.4]
- Add async consumer

## Version [6.0.3]
- Fix strong typing for data

## Version [6.0.2]
- Fix problem rich text

## Version [6.0.1]
- Initial release

## Version [6.0.0]
- Major version update

## Version [5.0.4]
- Update package version and dependency

## Version [5.0.3]
- Update package version and dependency

## Version [5.0.2]
- Update package version and dependency

## Version [5.0.1]
- Update package version and dependency

## Version [5.0.0]
- Major version update

## Version [4.4.0]
- Update package version and dependency

## Version [4.1.1]
- Update package version and dependency

## Version [4.1.0]
- Update package version and dependency

## Version [4.0.0]
- Major version update

## Version [3.6.1]
- Update package version and dependency

## Version [3.6.0]
- Update package version and dependency

## Version [3.4.1]
- Update package version and dependency

## Version [3.3.6]
- Update package version and dependency

## Version [3.3.4]
- Update package version and dependency

## Version [3.3.1]
- Update package version and dependency

## Version [3.3.0]
- Update package version and dependency

## Version [3.2.0]
- Update package version and dependency

## Version [3.1.1]
- Update package version and dependency

## Version [3.1.0]
- Update package version and dependency

## Version [3.0.0]
- Major version update

## Version [2.2.1]
- Update package version and dependency

## Version [2.1.4]
- Update package version and dependency

## Version [2.1.1]
- Update package version and dependency

## Version [2.1.0]
- Update package version and dependency

## Version [2.0.3]
- Update package version and dependency

## Version [2.0.2]
- Update package version and dependency

## Version [2.0.0]
- Major version update

## Version [1.5.1]
- Update package version and dependency

## Version [1.5.0]
- Update package version and dependency

## Version [1.4.0]
- Update package version and dependency

## Version [1.3.1]
- Update package version and dependency

## Version [1.3.0]
- Update package version and dependency

## Version [1.2.2]
- Update package version and dependency

## Version [1.2.1]
- Update package version and dependency

## Version [1.2.0]
- Adds method `CreateEntryForLocaleAsync` to the management client.

## Version [1.1.1]
- Adds missing properties for the `SystemProperties` object.

## Version [1.1.0]
- Adds convenience methods to `ContentfulManagementClient` for retrieving, creating and updating entries without having to use `Entry<dynamic>`.

## Version [1.0.0]
- Initial stable release on NetStandard 2.0
