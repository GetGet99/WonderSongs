namespace Get.UI.Data;
public delegate TTemplateParts ExternalControlTemplate<TTemplateParts, TIn>(TIn rootElement) where
    TTemplateParts : struct;
public delegate TTemplateParts ExternalControlTemplate<TTemplateParts, TControlType, TRootElement>(TControlType parent, TRootElement rootElement) where
    TTemplateParts : struct;