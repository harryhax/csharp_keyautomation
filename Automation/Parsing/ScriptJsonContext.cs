using System.Text.Json.Serialization;
using Csharp_GTA_KeyAutomation.Automation.Models;

namespace Csharp_GTA_KeyAutomation.Automation.Parsing;

/// <summary>
/// JSON source-generation context for automation scripts.
///
/// WHY THIS EXISTS:
/// ----------------
/// This project is published with trimming enabled (PublishTrimmed=true) to
/// reduce binary size. When trimming is enabled, reflection-based JSON
/// deserialization (e.g. JsonSerializer.Deserialize<T>) is unsafe because the
/// linker cannot statically determine which types must be preserved.
///
/// System.Text.Json normally relies on reflection to discover properties and
/// constructors at runtime. With trimming enabled, those members may be removed,
/// causing runtime failures or IL2026 warnings.
///
/// WHAT THIS DOES:
/// ---------------
/// This context enables *source-generated JSON serialization metadata* for
/// AutomationScriptFile and its entire object graph. The metadata is generated
/// at compile time, allowing the linker to safely preserve only the required
/// types and members.
///
/// BENEFITS:
/// ---------
/// - Eliminates IL2026 trim warnings
/// - Makes JSON deserialization fully trim-safe
/// - Reduces binary size
/// - Improves startup performance
/// - Avoids runtime reflection
///
/// IMPORTANT:
/// ----------
/// Do NOT remove this context unless trimming is disabled.
/// If new script model types are added (polymorphism, interfaces, inheritance),
/// additional JsonSerializable / JsonDerivedType attributes may be required.
/// </summary>


[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true
)]
[JsonSerializable(typeof(AutomationScriptFile))]
internal partial class ScriptJsonContext : JsonSerializerContext
{
}
