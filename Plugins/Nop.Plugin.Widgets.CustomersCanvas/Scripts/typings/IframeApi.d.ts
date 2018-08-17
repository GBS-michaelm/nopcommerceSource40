declare namespace LoadMask {
    function maskElement(element: HTMLElement): void;
    function maskMessage(element: HTMLElement, message: string, isError?: boolean): void;
    function unmaskElement(element: HTMLElement): void;
    function isMaskedElement(element: HTMLElement): boolean;
    function setCssUrl(url: string): void;
}
declare namespace CustomersCanvas.Utils.Ajax {
    const request: (parameters: IRequestParameters) => Promise<IResponse>;
    enum StatusCode {
        Ok = 200,
        NoContent = 204,
        InternalServerError = 500,
    }
    interface IRequestParameters {
        type: string;
        url: string;
        headers?: {
            name: string;
            value: string;
        }[];
        data?: string;
    }
    interface IResponse {
        value: string;
        status: number;
        contentType: string;
        xhr: XMLHttpRequest;
    }
    class Exception {
        response: IResponse;
        constructor(response: IResponse);
    }
    module RequestType {
        const post: string;
        const get: string;
    }
    module ContentType {
        function isJsonContentType(contentType: string): boolean;
        const json: {
            name: string;
            value: string;
        };
        const text: {
            name: string;
            value: string;
        };
    }
}
declare namespace CustomersCanvas.Utils.Type {
    function isStringArray(testData: any): testData is string[];
    function isObject(arg: any): boolean;
}
declare module CustomersCanvas.Utils {
    class UserAgent {
        static readonly gecko: boolean;
    }
}
declare module CustomersCanvas.Utils {
    class Deferred<T> {
        private _promise;
        private _resolve;
        private _reject;
        constructor();
        readonly promise: Promise<T>;
        resolve(value?: T | PromiseLike<T>): void;
        reject(error?: any): void;
    }
}
declare namespace CustomersCanvas {
    import Ajax = CustomersCanvas.Utils.Ajax;
    class Exception {
        errorData: any;
        name: string;
        message: string;
        stack: string;
        innerException: Error | Exception;
        constructor(msgOrException?: string | Error);
        toString(): string;
    }
    class NotImplementedException extends Exception {
        name: string;
        constructor(message?: string);
    }
    class AjaxResponseException extends Exception {
        ajaxResponse: Ajax.IResponse;
        name: string;
        constructor(ajaxResponse: Ajax.IResponse);
    }
    class ArgumentException extends Exception {
        name: string;
    }
}
declare namespace CustomersCanvas.IframeApi {
    namespace Exception {
        /** obsolete method */
        function create(msg?: string): Error;
        function isIframeApiException(ex: Error): boolean;
    }
    class ServerException extends CustomersCanvas.Exception {
        response: Utils.Ajax.IResponse;
        constructor(response: Utils.Ajax.IResponse);
    }
}
declare module CustomersCanvas.BaseEditor {
    enum GlobalEvents {
        ModalOpen = 0,
        ModalClose = 1,
        FullScreen = 2,
        FullWindow = 3,
        OriginalSize = 4,
        RevertProduct = 5,
    }
    enum WindowMode {
        FullScreen = 0,
        FullWindow = 1,
        OriginalSize = 2,
    }
}
declare module CustomersCanvas.IframeApi.PostMessage {
    class Events {
        static ModalOpen: string;
        static ModalClose: string;
        static FullWindow: string;
        static FullScreen: string;
        static OriginalSize: string;
        static RevertProduct: string;
        static BoundsNotification: string;
    }
    interface IProcCallDescription {
        id: number;
        clientInstanceId: number;
        methodName: string;
        methodArgs: any[];
    }
    enum MessageType {
        Event = 0,
        ProcedureResponse = 1,
    }
    interface IServerMessage {
        type: MessageType;
    }
    interface IProcResponse {
        id: number;
        result: any;
        success: boolean;
    }
    interface IServerEvent extends IServerMessage {
        event: string;
        result?: any[];
    }
}
declare namespace CustomersCanvas.IframeApi.PostMessage {
    class Client {
        private _targetFrame;
        private static _idOfLastProcedureCall;
        private static _idOfLastInstance;
        private _disposed;
        private _id;
        private _requestedMessages;
        private _listeners;
        private _onMessageDelegate;
        constructor(_targetFrame: HTMLIFrameElement);
        dispose(): void;
        callProcedure<TResult>(name: string, ...args: any[]): Promise<TResult>;
        subscribe(event: string, handler: (...args: any[]) => void): void;
        private _onMessage(ev);
        private _notifySubscribers(response);
        private _handleProcResponse(response);
    }
}
declare namespace CustomersCanvas.IframeApi.ObjectModel.Convert {
    function prepareForServer(definition: IProductDefinition): IProductDefinition;
    function fromPrintAreas(printAreas: (IPrintAreaTemplate | IPrintAreaDefinition)[]): IProductDefinition;
    function fromMockup(mockup: IMockupTemplate): IProductDefinition;
    function fromSurface(surface: SurfaceTypes): IProductDefinition;
}
declare module CustomersCanvas {
    const VERSION: string;
}
declare namespace CustomersCanvas.IframeApi {
    class FullWindowHandler {
        private _iframe;
        constructor(editor: Editor, _iframe: HTMLIFrameElement);
        private _setOriginalSize();
        private _setFullWindow();
        private _updateFullWindowStylesheet();
        private _getMaxZIndex();
    }
}
declare namespace CustomersCanvas.IframeApi.Configuration.RulersConfigUnit {
    type RulersConfigUnitType = "Mm" | "Inch" | "Cm" | "Point" | "Custom";
    const Mm: RulersConfigUnitType;
    const Inch: RulersConfigUnitType;
    const Cm: RulersConfigUnitType;
    const Point: RulersConfigUnitType;
    const Custom: RulersConfigUnitType;
}
declare namespace CustomersCanvas.IframeApi.Configuration.ModelMode {
    type ModelModeType = "Simple" | "Advanced" | "SimpleOnly";
    const Simple: ModelModeType;
    const Advanced: ModelModeType;
    const SimpleOnly: ModelModeType;
}
declare namespace CustomersCanvas.IframeApi.Configuration.FontListMode {
    type FontListModeType = "Simple" | "Advanced";
    const Simple: FontListModeType;
    const Advanced: FontListModeType;
}
declare namespace CustomersCanvas.IframeApi.Configuration.FinishButtonMode {
    type FinishButtonModeType = "Disabled" | "Download" | "Post";
    const Disabled: FinishButtonModeType;
    const Download: FinishButtonModeType;
    const Post: FinishButtonModeType;
}
declare namespace CustomersCanvas.IframeApi.Configuration.GalleryDialogTab {
    type GalleryDialogTabType = "User" | "Public" | "Facebook" | "Instagram";
    const User: GalleryDialogTabType;
    const Public: GalleryDialogTabType;
    const Facebook: GalleryDialogTabType;
    const Instagram: GalleryDialogTabType;
}
declare namespace CustomersCanvas.IframeApi.Configuration.BarcodeType {
    type BarcodeItemType = "QrVCard" | "QrPhone" | "QrUrl" | "LinearEan8" | "LinearEan13";
    const QrVCard: BarcodeItemType;
    const QrPhone: BarcodeItemType;
    const QrUrl: BarcodeItemType;
    const LinearEan8: BarcodeItemType;
    const LinearEan13: BarcodeItemType;
}
declare namespace CustomersCanvas.IframeApi.Configuration.ObjectInspectorPosition {
    type ObjectInspectorPositionType = "Left" | "Right";
    const Left: ObjectInspectorPositionType;
    const Right: ObjectInspectorPositionType;
}
declare namespace CustomersCanvas.IframeApi.Configuration {
    interface IFinishButtonConfig {
        mode: FinishButtonMode.FinishButtonModeType;
    }
    interface IBottomToolbarConfig {
        fullWindowButtonEnabled?: boolean;
        fullScreenButtonEnabled?: boolean;
        zoomValueEnabled?: boolean;
        zoomButtonsEnabled?: boolean;
        rotateButtonEnabled?: boolean;
        safetyLinesCheckboxEnabled?: boolean;
        gridCheckboxEnabled?: boolean;
        surfaceSwitchEnabled?: boolean;
        snapLinesCheckboxEnabled?: boolean;
    }
    interface ICommonConfig {
        fontSize?: IFontParams;
        tracking?: IFontParams;
        leading?: IFontParams;
    }
    interface ILeftToolbarConfig {
        imageButtonEnabled?: boolean;
        textButtonEnabled?: boolean;
        lineButtonEnabled?: boolean;
        rectangleButtonEnabled?: boolean;
        ellipseButtonEnabled?: boolean;
        linearBarcodeButtonEnabled?: boolean;
        qrCodeButtonEnabled?: boolean;
        richTextButtonEnabled?: boolean;
        disabled?: boolean;
    }
    interface IObjectInspectorConfig {
        bgItemEnabled?: boolean;
        dndEnabled?: boolean;
        qualityMeterEnabled?: boolean;
        safetyLineViolationWarningEnabled?: boolean;
        variableItemsEnabled?: boolean;
        emptyListTextEnabled?: boolean;
        position?: ObjectInspectorPosition.ObjectInspectorPositionType;
    }
    interface IItemMenuConfig {
        objectManipulationEnabled?: boolean;
        verticalAlignmentEnabled?: boolean;
        horizontalAlignmentEnabled?: boolean;
        changeZOrderEnabled?: boolean;
    }
    interface IFontParams {
        min?: number;
        max?: number;
        step?: number;
    }
    interface ITopToolbarConfig {
        alignButtonsEnabled?: boolean;
        historyButtonsEnabled?: boolean;
        revertButtonEnabled?: boolean;
        zOrderButtonsEnabled?: boolean;
        cloneButtonEnabled?: boolean;
        textAlignmentButtonsEnabled?: boolean;
        textEmphasisButtonsEnabled?: boolean;
        textFontFamilyButtonEnabled?: boolean;
        textFontStyleButtonEnabled?: boolean;
        textFontSizeButtonEnabled?: boolean;
        textFontColorButtonEnabled?: boolean;
        closeFontMenuOnClickEnabled?: boolean;
        opacitySliderEnabled?: boolean;
        textLeadingButtonEnabled?: boolean;
        textTrackingButtonEnabled?: boolean;
        multiRowModeEnabled?: boolean;
        borderColorButtonEnabled?: boolean;
        fontPreviewSize?: number;
        fontSize?: IFontParams;
    }
    interface IBarcodeDialogConfig {
        defaultType: BarcodeType.BarcodeItemType;
    }
    interface ITextOutline {
        enabled?: boolean;
        hueThreshold?: number;
        brightnessThreshold?: number;
        color?: string;
    }
    interface IRichComboValuesConfig {
        indent?: IFontParams;
        padding?: IFontParams;
    }
    interface IZoomConfig {
        enabled?: boolean;
        minZoomPct?: number;
        maxZoomPct?: number;
        autoZoom?: IAutoZoomConfig;
    }
    interface IAutoZoomConfig {
        enabled?: boolean;
        minFontSizeThresholdPt?: number;
        maxFontSizeThresholdPt?: number;
    }
    interface IRichTextDialogConfig {
        forcePasteAsPlainText: boolean;
        bgColor?: string;
        createMultiColumnText?: boolean;
        textOutline?: ITextOutline;
        toolbarConfig?: ({
            name: string;
            items: string[];
        } | string)[];
        zoom?: IZoomConfig;
        richComboValues?: IRichComboValuesConfig;
    }
    interface ISocialNetworkAppIds {
        instagram?: string;
        facebook?: string;
    }
    interface IGalleryDialogConfig {
        defaultTab?: GalleryDialogTab.GalleryDialogTabType;
        allowedExtensions?: string[];
        publicTabEnabled?: boolean;
        userTabEnabled?: boolean;
        publicFolderName?: string;
        overwriteExistingFiles?: boolean;
        showTabsInRestrictedMode?: boolean;
        socialNetworkAppIds?: ISocialNetworkAppIds;
    }
    interface IColorPickerConfig {
        showPaletteOnly?: boolean;
        palette?: string[][];
    }
    interface IImageEditorDialogConfig {
        colorAdjustButtonEnabled?: boolean;
        cropButtonEnabled?: boolean;
    }
    interface ITextPopupConfig {
    }
}
declare namespace CustomersCanvas.IframeApi {
    namespace Configuration {
        interface IRenderingConfig {
            hiResOutputToSeparateFiles?: boolean;
            hiResOutputDpi?: number;
            hiResOutputFileFormat?: string;
            hiResOutputColorSpace?: string;
            hiResOutputBackgroundColor?: string;
            hiResOutputCompression?: string;
            hiResOutputJpegCompressionQuality?: number;
            hiResOutputFlipMode?: string;
            proofImageFileFormat?: string;
            proofImageSafetyLinesEnabled?: boolean;
            proofImageCropSafetyLine?: string;
            proofImageWatermarkEnabled?: boolean;
            proofImageWatermarkFontPostScriptName?: string;
            proofImageWatermarkFontSize?: number;
            proofImageWatermarkText?: string;
            proofImageFlipMode?: string;
            proofImageSpineAndFoldingLinesEnabled?: boolean;
        }
        interface IGridConfig {
            horizontalColor?: string;
            verticalColor?: string;
            stepX?: number;
            stepY?: number;
            lineWidthPx?: number;
            enabled?: boolean;
        }
        interface ISnapLinesConfig {
            enabled?: boolean;
            color?: string;
            tolerance?: number;
        }
        interface IRulersConfig {
            enabled?: boolean;
            unit?: RulersConfigUnit.RulersConfigUnitType;
            customUnitScale?: number;
            origin?: {
                X: number;
                Y: number;
            };
            getUnitScale(): number;
        }
        interface ICanvasConfig {
            containerColor?: string;
            color?: string;
            shadowEnabled?: boolean;
            paddingPct?: number;
            zoomStep?: number;
            pinchZoomEnabled?: boolean;
            rulers?: IRulersConfig;
            snapLines?: ISnapLinesConfig;
        }
        interface IWidgetsConfig {
            common?: ICommonConfig;
            FinishButton?: IFinishButtonConfig;
            BottomToolbar?: IBottomToolbarConfig;
            LeftToolbar?: ILeftToolbarConfig;
            ObjectInspector?: IObjectInspectorConfig;
            ItemMenu?: IItemMenuConfig;
            TopToolbar?: ITopToolbarConfig;
            QrCodeDialog?: IBarcodeDialogConfig;
            LinearBarcodeDialog?: IBottomToolbarConfig;
            RichTextDialog?: IRichTextDialogConfig;
            GalleryDialog?: IGalleryDialogConfig;
            ColorPicker?: IColorPickerConfig;
            ImageEditorDialog?: IImageEditorDialogConfig;
        }
        interface IPerSurfaceConfiguration {
            [surfaceName: string]: {
                widgets?: IWidgetConfigsForSurface;
            };
        }
        interface IWidgetConfigsForSurface {
            LeftToolbar?: ILeftToolbarConfig;
            ObjectInspector?: IObjectInspectorConfig;
            ColorPicker?: ILeftToolbarConfig;
        }
        interface IClientConfiguration {
            defaultLanguage?: string;
            canvas?: ICanvasConfig;
            grid?: IGridConfig;
            alignToSafetyLineName?: string;
            newImageName?: string;
            initialMode?: ModelMode.ModelModeType;
            loadUserInfoButtonEnabled?: boolean;
            autoLoadUserInfo?: boolean;
            imageEditorEnabled?: boolean;
            deleteItemConfirmationEnabled?: boolean;
            revertProductConfirmationEnabled?: boolean;
            restoreProductOnReloadEnabled?: boolean;
            spellCheckEnabled?: boolean;
            variableItemsMaskSymbol?: string;
            fontListMode?: FontListMode.FontListModeType;
            widgets?: IWidgetsConfig;
            galleryOnly?: boolean;
            rendering?: IRenderingConfig;
            arbitraryResizeForImageItemEnabled?: boolean;
            perSurfaceConfiguration?: IPerSurfaceConfiguration;
        }
        /**
 * The editor configuration.
 * @structure
 * <pre class="autoCode">
 * <span class="src">IFrameAPI.js#IConfiguration</span>
 * </pre>
 */
        interface IConfiguration extends IClientConfiguration {
            userId?: string;
            userInfo?: {
                [key: string]: string;
            };
            customStyle?: string;
            fontList?: string[] | {
                systemFonts?: string[];
                appFonts?: string[];
            };
            theme?: string;
            preloader?: {
                enabled?: boolean;
                errorMessage?: string;
                firstTimeMessage?: string;
            };
            [key: string]: any;
        }
    }
}
declare namespace CustomersCanvas.IframeApi {
    import Client = CustomersCanvas.IframeApi.PostMessage.Client;
    class Editor {
        private _postMessageClient;
        private _context;
        static readonly version: string;
        constructor(iframe: HTMLIFrameElement, postMessageClient: Client, editorUrl: string, config?: Configuration.IConfiguration);
        getProofImages(options?: {
            maxWidth?: number;
            maxHeight?: number;
        }): Promise<Editor.IProofResult>;
        finishProductDesign(options?: {
            proofMaxWidth?: number;
            proofMaxHeight?: number;
            fileName?: string;
        }): Promise<Editor.IFinishDesignResult>;
        saveProduct(): Promise<Editor.ISaveProductResult>;
        revertProduct(revertToinitial?: boolean): Promise<void>;
        loadUserInfo(data?: Object): Promise<void>;
        getUnchangedItems(): Promise<Editor.IUnchangedItems>;
        subscribe(event: string, handler: (...args: any[]) => void): void;
        getProduct(): Promise<ObjectModel.Product>;
        eval(code: string | Function, ...args: any[]): Promise<any>;
        dispose(): void;
        private _checkParams(paramsDescription);
    }
    namespace Editor {
        interface IProofResult {
            proofImageUrls: string[][];
        }
        interface IFinishDesignResult {
            proofImageUrls: string[][];
            hiResOutputUrls: string[];
            returnToEditUrl: string;
            stateId: string;
            userId: string;
            userChanges: IUserChanges;
            boundsData: {
                currentSurface: {
                    Left: number;
                    Top: number;
                    Width: number;
                    Height: number;
                };
            };
        }
        interface IUserChanges {
            texts: {
                name: string;
                usersValue: string;
            }[];
            inStringPlaceholders: {
                name: string;
                usersValue: string;
            }[];
        }
        interface ISaveProductResult {
            stateId: string;
            userId: string;
            returnToEditUrl: string;
        }
        interface IContext {
            editor: Editor;
            client: Client;
            config: Configuration.IConfiguration;
            editorUrl: string;
        }
        interface IUnchangedItems {
            items: {
                name: string;
                surface: number;
            }[];
        }
    }
}
declare namespace CustomersCanvas.IframeApi.ObjectModel {
    type SurfaceTypes = string | IPrintAreaTemplate | ISurfaceDefinition | ISurfaceTemplate;
    interface IModelComponent {
        name?: string;
    }
    interface IProductDefinition extends IModelComponent {
        defaultSafetyLines?: ISafetyLine[];
        defaultDesignLocation?: IPointF;
        surfaces: SurfaceTypes[] | ISurfacesFromFolder;
    }
    interface IBaseSurfaceDefinition {
        foldingLines?: IFoldingLine[];
        spines?: ISpine[];
        proofImage?: IProofImage;
    }
    interface ISurfaceDefinition extends IBaseSurfaceDefinition, IModelComponent {
        width: number;
        height: number;
        printAreas?: IPrintAreaDefinition[];
    }
    interface ISurfaceTemplate extends IBaseSurfaceDefinition, IModelComponent {
        mockup?: IMockupTemplate;
        printAreas?: IPrintAreaTemplate[];
    }
    interface ISurfacesFromFolder extends IBaseSurfaceDefinition {
        designFolder?: string;
        mockupFolder?: string;
        previewMockupFolder?: string;
        upMockup?: boolean;
    }
    interface IFoldingLine {
        isVertical: boolean;
        margin: string;
        bleed: number;
        pen?: {
            width?: number;
            dashStep?: number;
            color?: string;
            altColor?: string;
        };
    }
    interface ISpine extends IFoldingLine {
        width: number;
        fillColor: string;
    }
    interface IBasePrintAreaDefinition extends IModelComponent {
        safetyLines?: ISafetyLine[];
        hiResOutput?: IHiResOutput;
    }
    interface IPrintAreaDefinition extends IBasePrintAreaDefinition {
        bounds: IRectangleF;
    }
    interface IPrintAreaTemplate extends IBasePrintAreaDefinition {
        designFile: string;
        designLocation?: IPointF;
    }
    interface IMockupTemplate extends IModelComponent {
        down?: string | {
            mockupFile: string;
            previewMockupFiles?: string[];
        };
        up?: string | {
            mockupFile: string;
            previewMockupFiles?: string[];
        };
    }
    interface ISafetyLine {
        name?: string;
        margin?: number;
        color?: string;
        altColor?: string;
        stepPx?: number;
        widthPx?: number;
        leftRightMargin?: number;
        topBottomMargin?: number;
    }
    interface IProofImage {
        fileFormat?: string;
        safetyLinesEnabled?: boolean;
        cropSafetyLineName?: string;
        watermarkEnabled?: boolean;
        watermarkFontPostScriptName?: string;
        watermarkFontSize?: number;
        watermarkText?: string;
        flipMode?: string;
        spineAndFoldingLinesEnabled?: boolean;
    }
    interface IHiResOutput {
        dpi?: number;
        fileFormat?: string;
        colorSpace?: string;
        backgroundColor?: string;
        compression?: string;
        jpegCompressionQuality?: number;
        flipMode?: string;
    }
    class ModelComponent {
        id: string;
        name: string;
        constructor(rawComponent: {});
    }
    class Product extends ModelComponent {
        private _context;
        surfaces: Surface[];
        userId: string;
        currentSurface: Surface;
        constructor(rawProduct: {}, _context: Editor.IContext);
        addSurface(surface: SurfaceTypes): Promise<Product>;
        removeSurface(surface: Surface): Promise<Product>;
        switchTo(surface: Surface): Promise<Product>;
        setUserId(userId: string): Promise<Product>;
    }
    class Surface extends ModelComponent {
        private _context;
        width: number;
        height: number;
        mockup: SurfaceMockup;
        constructor(rawSurface: {}, _context: Editor.IContext);
        setMockup(mockup: IMockupTemplate, options?: {
            updateRevertData?: boolean;
        }): Promise<Product>;
        setPrintAreas(printAreas: (IPrintAreaTemplate | IPrintAreaDefinition)[], options?: {
            updateRevertData?: boolean;
            preserveUserChanges?: boolean;
        }): Promise<Product>;
    }
    class SurfaceMockup extends ModelComponent {
        constructor(rawMockup: {});
    }
    interface IPointF {
        x: number;
        y: number;
    }
    interface IRectangleF {
        x: number;
        y: number;
        width: number;
        height: number;
    }
    interface ISize {
        width: number;
        height: number;
    }
}
import Client = CustomersCanvas.IframeApi.PostMessage.Client;
declare namespace CustomersCanvas.IframeApi {
    class EditorLoader {
        private _iframe;
        private _editorData;
        private static _existingLoaders;
        private _config;
        private _editorUrl;
        private _status;
        private _iframeClient;
        constructor(_iframe: HTMLIFrameElement, _editorData: EditorLoader.IEditorData | QueryString, editorUrl: string);
        start(onFirstLoad?: () => void): Promise<Editor>;
        dispose(): void;
        private _queryStringToConfigConverter(queryString);
        private _waitFirefoxReady();
        private _getConfigFromServer(onFirstLoad?);
        static getConfigFromServer(editorUrl: string, productDefinition: StateId[] | ObjectModel.IProductDefinition, configFromApi: Configuration.IConfiguration, onFirstConfig?: () => void): Promise<string>;
        static getConfigFromServerByQueryString(editorUrl: string, queryString: string, onFirstConfig?: () => void): Promise<string>;
        private _checkLoadStatus();
        private _normalizeUrl(url);
        private _cleanUpIframe();
        private _loadIframe(url);
        private _getIframeUrl();
        private _initEditor(configFromServer);
        private _handleServerException(ex, initClient);
    }
    namespace EditorLoader {
        enum Status {
            Initialized = 0,
            Started = 1,
            Finished = 2,
            Disposed = 3,
            Error = 4,
        }
        interface IEditorData {
            productData: StateId[] | ObjectModel.IProductDefinition;
            config?: Configuration.IConfiguration;
        }
        interface IConfigRequestData {
            productDefinition?: ObjectModel.IProductDefinition;
            states?: string[];
            userId?: string;
            userInfo?: {
                [key: string]: string;
            };
            configFromApi: Object;
            type: string;
        }
        interface IFirstConfigRequest extends IConfigRequestData {
            maxThumbnailWidth?: number;
            maxThumbnailHeight: number;
            type: string;
        }
        class LoadingStoppedException extends CustomersCanvas.Exception {
        }
    }
}
declare namespace CustomersCanvas.IframeApi {
    let editorUrl: string;
    function loadEditor(editorFrame: HTMLIFrameElement, product: string | string[] | ObjectModel.IProductDefinition, config?: Configuration.IConfiguration, onFirstLoad?: () => void): Promise<Editor>;
    function loadEditorByQueryString(editorFrame: HTMLIFrameElement, queryString: string, onFirstLoad?: () => void): Promise<Editor>;
    function _createPreConfiguredEditor(editorFrame: HTMLIFrameElement): Editor;
    function _waitElementVisible(element: HTMLElement): Promise<void>;
    class QueryString {
        value: string;
        constructor(value: string);
        getDeserializedObject(): {
            key: string;
            value: string;
        }[];
    }
    class StateId {
        value: string;
        constructor(value: string);
    }
}
