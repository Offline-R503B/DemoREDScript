// Single line comment

/*
 * Multi-line comment
*/

enum flibSortOrder {
  Timestamp = 0,
  Name = 1,
  Difficulty = 2 /* This comment shouldn't affect the rest of the line */
}

public class flibSortingUtils {
  public static func GetButtonEventName() -> CName = n"toggle_comparison_tooltip"

  // Returns the Localization Key CName for a given sorting order
  public static func GetSortOrderLocKey(order: flibSortOrder) -> CName {
    switch order {
      case flibSortOrder.Timestamp:
        return n"UI-Labels-Time";
      case flibSortOrder.Name:
        return n"UI-Sorting-Name";
      case flibSortOrder.Difficulty:
        return n"UI-ResourceExports-Threat";
      default:
        // Shouldn't ever hit this, but the scripting runtime
        // supports undefined values for enums so ¯\_(ツ)_/¯
        return n"UI-Sorting-Default";
    }
  }

  public static func GetSortOrderButtonHint(order: flibSortOrder) -> String {
    // Took a while to find a lockey that said anything related to sorting
    // This one just says "Sorting" in lang_en
    let sortLabel = GetLocalizedTextByKey(n"Story-base-gameplay-gui-fullscreen-inventory-backpack-_localizationString0");
    let orderStr = GetLocalizedTextByKey(flibSortingUtils.GetSortOrderLocKey(order));

    return sortLabel + ": " + orderStr;
  }
}

@addMethod(QuestMappinLinkController)
public func flib_SetupVehicleIcon(questId: String) -> Void {
  let iconRecord: ref<UIIcon_Record> = flib_GetVehicleIcon(questId);
  if IsDefined(iconRecord) {
    inkImageRef.SetAtlasResource(this.m_linkImage, iconRecord.AtlasResourcePath());
    inkImageRef.SetTexturePart(this.m_linkImage, iconRecord.AtlasPartName());
  }
}

// Added field to track the group each shard belongs to
@addField(ShardEntryData)
public let f_group: wref<ShardEntryData>;

public func flib_TestCode() -> Void {
  let var1: Int32 = condition2 ? 42 : 69;
  let var2: TweakDBID = t"UIJournalIcons.v_sportbike2_arch_player";
  let var3: array<Float> = [ 1.0, 3.14159, 123.456 ];
  let var4: Double = 456.789d;

  if condition {
    this.fn();
  }
  else {
    super.parent_method();
  }

  while true {
    
  }
}
