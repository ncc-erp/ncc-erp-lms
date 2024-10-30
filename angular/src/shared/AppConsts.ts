export class AppConsts {

    static remoteServiceBaseUrl: string;
    static appBaseUrl: string;
    static appBaseHref: string; // returns angular's base-href parameter value if used during the publish

    static localeMappings: any = [];

    static readonly userManagement = {
        defaultAdminUserName: 'admin'
    };

    static readonly localization = {
        defaultLocalizationSourceName: 'RMALMS'
    };

    static readonly authorization = {
        encrptedAuthTokenName: 'enc_auth_token'
    };

    static reCaptcha = {
        siteKey: '6LeIxAcTAAAAAJcZVRqyHh71UMIEGNQ_MXjiZKhI', // Register: www.google.com/recaptcha/admin/create => change in assets\appconfig.prod.json
        loginCount: 3 // count show reCaptcha when login failse; = 100 => Always hide; 0 => Always show
    };

    static readonly Tinymceplugins = `print preview fullpage searchreplace autolink directionality visualblocks
    visualchars fullscreen image link media template codesample table charmap hr
    pagebreak nonbreaking anchor toc insertdatetime advlist
    lists textcolor wordcount
    imagetools contextmenu colorpicker textpattern help preview`

    static readonly Tinymcetoolbar = `formatselect | fontselect  | bold italic strikethrough forecolor backcolor | fontsizeselect | link  image| alignleft aligncenter alignright alignjustify  | numlist bullist outdent indent  | removeformat | preview `

    static readonly TinymceFont = `Andale Mono=andale mono,times; Arial=arial,helvetica,sans-serif; Arial Black=arial black,avant garde; Book Antiqua=book antiqua,palatino; Comic Sans MS=comic sans ms,sans-serif; Courier New=courier new,courier; Georgia=georgia,palatino; Helvetica=helvetica; Impact=impact,chicago; Oswald=oswald; Symbol=symbol; Tahoma=tahoma,arial,helvetica,sans-serif; Terminal=terminal,monaco; Times New Roman=times new roman,times; Trebuchet MS=trebuchet ms,geneva; Verdana=verdana,geneva; Webdings=webdings; Wingdings=wingdings,zapf dingbats`

}
