import { ActionType, AsyncValidatorType, ChartType, ClaimValueType, ControlType, DatasourceControlType, FieldValueType, FilterType, PageSectionLayoutType, SectionContructionType, StandardType, ValidatorType } from 'services/portal.service'

export class StaticResources {

    public static asyncValidatorTypes() {
        return [
            { name: 'Database Validator', value: AsyncValidatorType.DatabaseValidator },
            { name: 'Http Validator', value: AsyncValidatorType.HttpValidator }
        ]
    }

    public static standardTypes(){
        return [
            { name: 'Standard', value: StandardType.Standard },
            { name: 'Array', value: StandardType.Array },
            { name: 'Tree', value: StandardType.Tree },
        ]
    }

    public static claimValueTypes() {
        return [
            { name: 'Boolean', value: ClaimValueType.Boolean },
            { name: 'String', value: ClaimValueType.String },
            { name: 'Number', value: ClaimValueType.Number },
            { name: 'Array', value: ClaimValueType.Array }
        ]
    }

    public static formValidatorTypes() {
        return [
            { name: 'Required', value: ValidatorType.Required, validatorName: 'required' },
            { name: 'Min Length', value: ValidatorType.MinLength, validatorName: 'minlength' },
            { name: 'Max Length', value: ValidatorType.MaxLength, validatorName: 'maxlength' },
            { name: 'Regex', value: ValidatorType.Regex, validatorName: 'pattern' },
            { name: 'Number', value: ValidatorType.Number, validatorName: 'number' },
            { name: 'Number Range', value: ValidatorType.NumberRange, validatorName: 'range' },
            { name: 'Max Date', value: ValidatorType.MaxDate, validatorName: 'maxDate' },
            { name: 'Min Date', value: ValidatorType.MinDate, validatorName: 'minDate' },
            { name: 'Date', value: ValidatorType.DateTime, validatorName: 'date' },
            { name: 'Email', value: ValidatorType.Email, validatorName: 'email' },
            { name: 'Equal', value: ValidatorType.Equal, validatorName: 'equal' },
            { name: 'EqualTo', value: ValidatorType.EqualTo, validatorName: 'equalTo' },
            { name: 'Json', value: ValidatorType.Json, validatorName: 'json' },
            { name: 'File Maximum Size', value: ValidatorType.FileMaximumSize, validatorName: 'maximumsize' },
            { name: 'File Extensions', value: ValidatorType.FileExtensions, validatorName: 'fileextensions' },
            { name: 'Number of files', value: ValidatorType.FileMaximumFiles, validatorName: 'numberfiles' }
        ]
    }

    public static commandPositionTypes() {
        return [
            { name: 'In List', value: 0 },
            { name: 'Outside', value: 1 }
        ]
    }

    public static actionTypes() {
        return [
            { name: 'Execute Database', value: ActionType.ExecuteDatabase },
            { name: 'Http Service', value: ActionType.CallHttpService },
            { name: 'Redirect', value: ActionType.Redirect }
        ]
    }

    public static paramSourceTypes() {
        return [
        ]
    }

    public static controlTypes() {
        return [
            { name: 'LineBreaker', value: ControlType.LineBreaker },
            { name: 'Label', value: ControlType.Label },
            { name: 'Text box', value: ControlType.Textbox },
            { name: 'Textarea', value: ControlType.Textarea },
            { name: 'Email', value: ControlType.Email },
            { name: 'Number', value: ControlType.Number },
            { name: 'Date Picker', value: ControlType.DateTime },
            { name: 'Checkbox', value: ControlType.Checkbox },
            { name: 'Slide', value: ControlType.Slide },
            { name: 'Radio', value: ControlType.Radio },
            { name: 'Select', value: ControlType.Select },
            { name: 'Auto Complete', value: ControlType.AutoComplete },
            { name: 'Rich Text Editor', value: ControlType.RichTextEditor },
            { name: 'Uploader', value: ControlType.Uploader },
            { name: 'MultiUploader', value: ControlType.MultiUploader },
            { name: 'Icon Picker', value: ControlType.IconPicker },
            { name: 'Markdown Editor', value: ControlType.MarkdownEditor },
            { name: 'Composite Control', value: ControlType.Composite }
        ];
    }

    public static constructionTypes() {
        return [
            { name: 'Standard', value: SectionContructionType.Standard },            
            { name: 'Array', value: SectionContructionType.Array },
            { name: 'Dynamic List', value: SectionContructionType.DynamicList },
            { name: 'Chart', value: SectionContructionType.Chart },
            { name: 'Tree', value: SectionContructionType.Tree }
        ]
    }

    public static sectionLayoutTypes() {
        return [
            { name: 'One Column', value: PageSectionLayoutType.OneColumn },
            { name: 'Two Column', value: PageSectionLayoutType.TwoColumns },
            { name: 'Four Columns', value: PageSectionLayoutType.FourColumns },
            { name: 'Six Columns', value: PageSectionLayoutType.SixColumns }
        ]
    }

    public static httpCallMethods() {
        return [
            { name: 'GET', value: 'Get' },
            { name: 'POST', value: 'Post' },
            { name: 'PUT', value: 'Put' },
            { name: 'DELETE', value: 'Delete' }
        ]
    }

    public static colorButtons() {
        return [
            { name: 'primary', value: 'primary' },
            { name: 'warn', value: 'warn' },
            { name: 'basic', value: 'basic' },
            { name: 'accent', value: 'accent' }
        ]
    }

    public static datasourceTypes() {
        return [
            { name: 'Static Resource', value: DatasourceControlType.StaticResource },
            { name: 'Database', value: DatasourceControlType.Database },
            { name: 'Web service', value: DatasourceControlType.WebService }
        ]
    }

    public static fieldValueTypes() {
        return [
            { name: 'Textbox', value: FieldValueType.Text },
            { name: 'Number', value: FieldValueType.Number },
            { name: 'Date Picker', value: FieldValueType.DatePicker },
            { name: 'Checkbox', value: FieldValueType.Checkbox },
            { name: 'Slide', value: FieldValueType.Slide },
            { name: 'Select', value: FieldValueType.Select },
        ]
    }

    public static chartTypes(){
        return [
            { name: 'Vertical Bar Chart', value: ChartType.VerticalBarChart },
            { name: 'Horizontal Bar Chart', value: ChartType.HorizontalBarChart },
            { name: 'Grouped Vertical Bar Chart', value: ChartType.GroupedVerticalBarChart },
            { name: 'Grouped Horizontal Bar Chart', value: ChartType.GroupedHorizontalBarChart },
            { name: 'Pie Chart', value: ChartType.PieChart },
            { name: 'Advanced Pie Chart', value: ChartType.AdvancedPieChart },
            { name: 'Pie Grid', value: ChartType.PieGrid },
            { name: 'Line Chart', value: ChartType.LineChart },
            { name: 'Area Chart', value: ChartType.AreaChart },
            { name: 'Gauge', value: ChartType.Gauge },
            { name: 'Number Card', value: ChartType.NumberCard }
        ]
    }

    public static chartFilterTypes(){
        return [
           { name: 'staticResources.chartFilterType.checkbox', value: FilterType.Checkbox } ,
           { name: 'staticResources.chartFilterType.select', value: FilterType.Select } ,
           { name: 'staticResources.chartFilterType.numberPicker', value: FilterType.NumberPicker } ,
           { name: 'staticResources.chartFilterType.datePicker', value: FilterType.DatePicker } ,
           { name: 'staticResources.chartFilterType.monthYearPicker', value: FilterType.MonthYearPicker }
        ]
    }

    public static localeTags(){
        return [
            { name: 'Arabic, Saudi Arabia', value: 'ar-SA' },
            { name: 'Bulgarian, Bulgaria', value: 'bg-BG' },
            { name: 'Catalan, Spain', value: 'ca-ES' },
            { name: 'Chinese, Taiwan, Province of China', value: 'zh-TW' },
            { name: 'Czech, Czech Republic', value: 'cs-CZ' },
            { name: 'Danish, Denmark', value: 'da-DK' },
            { name: 'German, Germany', value: 'de-DE' },
            { name: 'Modern Greek (1453-), Greece', value: 'el-GR' },
            { name: 'English, United States', value: 'en-US' },
            { name: 'Spanish', value: 'es-ES_tradnl' },
            { name: 'Finnish, Finland', value: 'fi-FI' },
            { name: 'French, France', value: 'fr-FR' },
            { name: 'Hebrew, Israel', value: 'he-IL' },
            { name: 'Hungarian, Hungary', value: 'hu-HU' },
            { name: 'Icelandic, Iceland', value: 'is-IS' },
            { name: 'Italian, Italy', value: 'it-IT' },
            { name: 'Japanese, Japan', value: 'ja-JP' },
            { name: 'Korean, Republic of Korea', value: 'ko-KR' },
            { name: 'Dutch, Netherlands', value: 'nl-NL' },
            { name: 'Norwegian Bokmål, Norway', value: 'nb-NO' },
            { name: 'Polish, Poland', value: 'pl-PL' },
            { name: 'Portuguese, Brazil', value: 'pt-BR' },
            { name: 'Romansh, Switzerland', value: 'rm-CH' },
            { name: 'Romanian, Romania', value: 'ro-RO' },
            { name: 'Russian, Russian Federation', value: 'ru-RU' },
            { name: 'Croatian, Croatia', value: 'hr-HR' },
            { name: 'Slovak, Slovakia', value: 'sk-SK' },
            { name: 'Albanian, Albania', value: 'sq-AL' },
            { name: 'Swedish, Sweden', value: 'sv-SE' },
            { name: 'Thai, Thailand', value: 'th-TH' },
            { name: 'Turkish, Turkey', value: 'tr-TR' },
            { name: 'Urdu, Pakistan', value: 'ur-PK' },
            { name: 'Indonesian, Indonesia', value: 'id-ID' },
            { name: 'Ukrainian, Ukraine', value: 'uk-UA' },
            { name: 'Belarusian, Belarus', value: 'be-BY' },
            { name: 'Slovenian, Slovenia', value: 'sl-SI' },
            { name: 'Estonian, Estonia', value: 'et-EE' },
            { name: 'Latvian, Latvia', value: 'lv-LV' },
            { name: 'Lithuanian, Lithuania', value: 'lt-LT' },
            { name: 'Tajik, Cyrillic, Tajikistan', value: 'tg-Cyrl-TJ' },
            { name: 'Persian, Islamic Republic of Iran', value: 'fa-IR' },
            { name: 'Vietnamese, Viet Nam', value: 'vi-VN' },
            { name: 'Armenian, Armenia', value: 'hy-AM' },
            { name: 'Azerbaijani, Latin, Azerbaijan', value: 'az-Latn-AZ' },
            { name: 'Basque, Spain', value: 'eu-ES' },
            { name: 'Sorbian languages, Germany', value: 'wen-DE' },
            { name: 'Macedonian, The Former Yugoslav Republic of Macedonia', value: 'mk-MK' },
            { name: 'Southern Sotho, South Africa', value: 'st-ZA' },
            { name: 'Tsonga, South Africa', value: 'ts-ZA' },
            { name: 'Tswana, South Africa', value: 'tn-ZA' },
            { name: 'South Africa', value: 'ven-ZA' },
            { name: 'Xhosa, South Africa', value: 'xh-ZA' },
            { name: 'Zulu, South Africa', value: 'zu-ZA' },
            { name: 'Afrikaans, South Africa', value: 'af-ZA' },
            { name: 'Georgian, Georgia', value: 'ka-GE' },
            { name: 'Faroese, Faroe Islands', value: 'fo-FO' },
            { name: 'Hindi, India', value: 'hi-IN' },
            { name: 'Maltese, Malta', value: 'mt-MT' },
            { name: 'Northern Sami, Norway', value: 'se-NO' },
            { name: 'Malay (macrolanguage), Malaysia', value: 'ms-MY' },
            { name: 'Kazakh, Kazakhstan', value: 'kk-KZ' },
            { name: 'Kirghiz, Kyrgyzstan', value: 'ky-KG' },
            { name: 'Swahili (macrolanguage), Kenya', value: 'sw-KE' },
            { name: 'Turkmen, Turkmenistan', value: 'tk-TM' },
            { name: 'Uzbek, Latin, Uzbekistan', value: 'uz-Latn-UZ' },
            { name: 'Tatar, Russian Federation', value: 'tt-RU' },
            { name: 'Bengali, India', value: 'bn-IN' },
            { name: 'Panjabi, India', value: 'pa-IN' },
            { name: 'Gujarati, India', value: 'gu-IN' },            
            { name: 'Oriya, India', value: 'or-IN' },
            { name: 'Tamil, India', value: 'ta-IN' },
            { name: 'Telugu, India', value: 'te-IN' },
            { name: 'Kannada, India', value: 'kn-IN' },
            { name: 'Malayalam, India', value: 'ml-IN' },
            { name: 'Assamese, India', value: 'as-IN' },
            { name: 'Marathi, India', value: 'mr-IN' },
            { name: 'Sanskrit, India', value: 'sa-IN' },
            { name: 'Mongolian, Mongolia', value: 'mn-MN' },
            { name: 'Tibetan, China', value: 'bo-CN' },
            { name: 'Welsh, United Kingdom', value: 'cy-GB' },
            { name: 'Central Khmer, Cambodia', value: 'km-KH' },
            { name: 'Lao, Lao People’s Democratic Republic', value: 'lo-LA' },
            { name: 'Burmese, Myanmar', value: 'my-MM' },
            { name: 'Galician, Spain', value: 'gl-ES' },
            { name: 'Konkani (macrolanguage), India', value: 'kok-IN' },
            { name: 'Manipuri', value: 'mni' },
            { name: 'Sindhi, India', value: 'sd-IN' },
            { name: 'Syriac, Syrian Arab Republic', value: 'syr-SY' },
            { name: 'Sinhala, Sri Lanka', value: 'si-LK' },
            { name: 'Cherokee, United States', value: 'chr-US' },
            { name: 'Inuktitut, Unified Canadian Aboriginal Syllabics, Canada', value: 'iu-Cans-CA' },
            { name: 'Amharic, Ethiopia', value: 'am-ET' },
            { name: 'Nepali, Nepal', value: 'ne-NP' },            
            { name: 'Western Frisian, Netherlands', value: 'fy-NL' },  
            { name: 'Pushto, Afghanistan', value: 'ps-AF' },  
            { name: 'Filipino, Philippines', value: 'fil-PH' },  
            { name: 'Dhivehi, Maldives', value: 'dv-MV' },  
            { name: 'Bini, Nigeria', value: 'bin-NG' },  
            { name: 'Nigerian Fulfulde, Nigeria', value: 'fuv-NG' },  
            { name: 'Hausa, Latin, Nigeria', value: 'ha-Latn-NG' },  
            { name: 'Ibibio, Nigeria', value: 'ibb-NG' },  
            { name: 'Yoruba, Nigeria', value: 'yo-NG' },  
            { name: 'Cusco Quechua, Bolivia', value: 'quz-BO' },  
            { name: 'Pedi, South Africa', value: 'nso-ZA' },
            { name: 'Bashkir, Russian Federation', value: 'ba-RU' },
            { name: 'Luxembourgish, Luxembourg', value: 'lb-LU' },
            { name: 'Kalaallisut, Greenland', value: 'kl-GL' },
            { name: 'Igbo, Nigeria', value: 'ig-NG' },
            { name: 'Kanuri, Nigeria', value: 'kr-NG' },
            { name: 'West Central Oromo, Ethiopia', value: 'gaz-ET' },
            { name: 'Tigrinya, Eritrea', value: 'ti-ER' },
            { name: 'Guarani, Paraguay', value: 'gn-PY' },
            { name: 'Hawaiian, United States', value: 'haw-US' },
            { name: 'Somali, Somalia', value: 'so-SO' },
            { name: 'Sichuan Yi, China', value: 'ii-CN' },
            { name: 'Papiamento, Netherlands Antilles', value: 'pap-AN' },            
            { name: 'Mapudungun, Chile', value: 'arn-CL' }, 
            { name: 'Mohawk, Canada', value: 'moh-CA' }, 
            { name: 'Breton, France', value: 'br-FR' }, 
            { name: 'Uighur, China', value: 'ug-CN' }, 
            { name: 'Maori, New Zealand', value: 'mi-NZ' }, 
            { name: 'Occitan (post 1500), France', value: 'oc-FR' }, 
            { name: 'Corsican, France', value: 'co-FR' },
            { name: 'Swiss German, France', value: 'gsw-FR' },
            { name: 'Yakut, Russian Federation', value: 'sah-RU' },
            { name: 'Guatemala', value: 'qut-GT' },
            { name: 'Kinyarwanda, Rwanda', value: 'rw-RW' },
            { name: 'Wolof, Senegal', value: 'wo-SN' },
            { name: 'Dari, Afghanistan', value: 'prs-AF' },
            { name: 'Plateau Malagasy, Madagascar', value: 'plt-MG' },
            { name: 'Scottish Gaelic, United Kingdom', value: 'gd-GB' }
        ]
    }

    public static iconsList() {
        return ['3d_rotation', 'ac_unit', 'access_alarm', 'access_alarms', 'access_time', 'accessibility', 'accessible', 'account_balance', 'account_balance_wallet',
            'account_box', 'account_circle', 'adb', 'add', 'add_a_photo', 'add_alarm', 'add_alert', 'add_box', 'add_circle', 'add_circle_outline',
            'add_location', 'add_shopping_cart', 'add_to_photos', 'add_to_queue', 'adjust', 'airline_seat_flat', 'airline_seat_flat_angled',
            'airline_seat_individual_suite', 'airline_seat_legroom_extra', 'airline_seat_legroom_normal', 'airline_seat_legroom_reduced',
            'airline_seat_recline_extra', 'airline_seat_recline_normal', 'airplanemode_active', 'airplanemode_inactive', 'airplay',
            'airport_shuttle', 'alarm', 'alarm_add', 'alarm_off', 'alarm_on', 'album', 'all_inclusive', 'all_out', 'android',
            'announcement', 'apps', 'archive', 'arrow_back', 'arrow_downward', 'arrow_drop_down', 'arrow_drop_down_circle', 'arrow_drop_up',
            'arrow_forward', 'arrow_upward', 'art_track', 'aspect_ratio', 'assessment', 'assignment', 'assignment_ind', 'assignment_late',
            'assignment_return', 'assignment_returned', 'assignment_turned_in', 'assistant', 'assistant_photo', 'attach_file', 'attach_money',
            'attachment', 'audiotrack', 'autorenew', 'av_timer', 'backspace', 'backup', 'battery_alert', 'battery_charging_full', 'battery_full', 'battery_std',
            'battery_unknown', 'beach_access', 'beenhere', 'block', 'bluetooth', 'bluetooth_audio', 'bluetooth_connected', 'bluetooth_disabled', 'bluetooth_searching',
            'blur_circular', 'blur_linear', 'blur_off', 'blur_on', 'book', 'bookmark', 'bookmark_border', 'border_all', 'border_bottom',
            'border_clear', 'border_color', 'border_horizontal', 'border_inner', 'border_left', 'border_outer', 'border_right', 'border_style',
            'border_top', 'border_vertical', 'branding_watermark', 'brightness_1', 'brightness_2', 'brightness_3', 'brightness_4', 'brightness_5', 'brightness_6',
            'brightness_7', 'brightness_auto', 'brightness_high', 'brightness_low', 'brightness_medium', 'broken_image', 'brush', 'bubble_chart', 'bug_report',
            'build', 'burst_mode', 'business', 'business_center', 'cached', 'cake', 'call', 'call_end', 'call_made', 'call_merge', 'call_missed', 'call_missed_outgoing',
            'call_received', 'call_split', 'call_to_action', 'camera', 'camera_alt', 'camera_enhance', 'camera_front', 'camera_rear', 'camera_roll', 'cancel',
            'card_giftcard', 'card_membership', 'card_travel', 'casino', 'cast', 'cast_connected', 'center_focus_strong', 'center_focus_weak', 'change_history',
            'chat', 'chat_bubble', 'chat_bubble_outline', 'check', 'check_box', 'check_box_outline_blank', 'check_circle', 'chevron_left', 'chevron_right',
            'child_care', 'child_friendly', 'chrome_reader_mode', 'class', 'clear', 'clear_all', 'close', 'closed_caption', 'cloud', 'cloud_circle', 'cloud_done',
            'cloud_download', 'cloud_off', 'cloud_queue', 'cloud_upload', 'code', 'collections', 'collections_bookmark', 'color_lens', 'colorize',
            'comment', 'compare', 'compare_arrows', 'computer', 'confirmation_number', 'contact_mail', 'contact_phone', 'contacts', 'content_copy',
            'content_cut', 'content_paste', 'control_point', 'control_point_duplicate', 'copyright', 'create', 'create_new_folder', 'credit_card', 'crop',
            'crop_16_9', 'crop_3_2', 'crop_5_4', 'crop_7_5', 'crop_din', 'crop_free', 'crop_landscape', 'crop_original', 'crop_portrait', 'crop_rotate', 'crop_square',
            'dashboard', 'data_usage', 'date_range', 'dehaze', 'delete', 'delete_forever', 'delete_sweep', 'description', 'desktop_mac', 'desktop_windows', 'details',
            'developer_board', 'developer_mode', 'device_hub', 'devices', 'devices_other', 'dialer_sip', 'dialpad', 'directions', 'directions_bike', 'directions_boat',
            'directions_bus', 'directions_car', 'directions_railway', 'directions_run', 'directions_subway', 'directions_transit', 'directions_walk', 'disc_full',
            'dns', 'do_not_disturb', 'do_not_disturb_alt', 'do_not_disturb_off', 'do_not_disturb_on', 'dock', 'domain', 'done', 'done_all', 'donut_large', 'donut_small',
            'drafts', 'drag_handle', 'drive_eta', 'dvr', 'edit', 'edit_location', 'eject', 'email', 'enhanced_encryption', 'equalizer', 'error', 'error_outline',
            'euro_symbol', 'ev_station', 'event', 'event_available', 'event_busy', 'event_note', 'event_seat', 'exit_to_app', 'expand_less', 'expand_more',
            'explicit', 'explore', 'exposure', 'exposure_neg_1', 'exposure_neg_2', 'exposure_plus_1', 'exposure_plus_2', 'exposure_zero', 'extension',
            'face', 'fast_forward', 'fast_rewind', 'favorite', 'favorite_border', 'featured_play_list', 'featured_video', 'feedback', 'fiber_dvr',
            'fiber_manual_record', 'fiber_new', 'fiber_pin', 'fiber_smart_record', 'file_download', 'file_upload', 'filter', 'filter_1', 'filter_2', 'filter_3',
            'filter_4', 'filter_5', 'filter_6', 'filter_7', 'filter_8', 'filter_9', 'filter_9_plus', 'filter_b_and_w', 'filter_center_focus', 'filter_drama', 'filter_frames',
            'filter_hdr', 'filter_list', 'filter_none', 'filter_tilt_shift', 'filter_vintage', 'find_in_page', 'find_replace', 'fingerprint', 'first_page', 'fitness_center',
            'flag', 'flare', 'flash_auto', 'flash_off', 'flash_on', 'flight', 'flight_land', 'flight_takeoff', 'flip', 'flip_to_back', 'flip_to_front', 'folder', 'folder_open',
            'folder_shared', 'folder_special', 'font_download', 'format_align_center', 'format_align_justify', 'format_align_left', 'format_align_right', 'format_bold',
            'format_clear', 'format_color_fill', 'format_color_reset', 'format_color_text', 'format_indent_decrease', 'format_indent_increase', 'format_italic',
            'format_line_spacing', 'format_list_bulleted', 'format_list_numbered', 'format_paint', 'format_quote', 'format_shapes', 'format_size', 'format_strikethrough',
            'format_textdirection_l_to_r', 'format_textdirection_r_to_l', 'format_underlined', 'forum', 'forward', 'forward_10', 'forward_30', 'forward_5', 'free_breakfast',
            'fullscreen', 'fullscreen_exit', 'functions', 'g_translate', 'gamepad', 'games', 'gavel', 'gesture', 'get_app', 'gif', 'golf_course', 'gps_fixed',
            'gps_not_fixed', 'gps_off', 'grade', 'gradient', 'grain', 'graphic_eq', 'grid_off', 'grid_on', 'group', 'group_add', 'group_work', 'hd', 'hdr_off',
            'hdr_on', 'hdr_strong', 'hdr_weak', 'headset', 'headset_mic', 'healing', 'hearing', 'help', 'help_outline', 'high_quality', 'highlight', 'highlight_off',
            'history', 'home', 'hot_tub', 'hotel', 'hourglass_empty', 'hourglass_full', 'http', 'https', 'image', 'image_aspect_ratio', 'import_contacts',
            'import_export', 'important_devices', 'inbox', 'indeterminate_check_box', 'info', 'info_outline', 'input', 'insert_chart', 'insert_comment',
            'insert_drive_file', 'insert_emoticon', 'insert_invitation', 'insert_link', 'insert_photo', 'invert_colors', 'invert_colors_off', 'iso', 'keyboard',
            'keyboard_arrow_down', 'keyboard_arrow_left', 'keyboard_arrow_right', 'keyboard_arrow_up', 'keyboard_backspace', 'keyboard_capslock', 'keyboard_hide',
            'keyboard_return', 'keyboard_tab', 'keyboard_voice', 'kitchen', 'label', 'label_outline', 'landscape', 'language', 'laptop', 'laptop_chromebook', 'laptop_mac',
            'laptop_windows', 'last_page', 'launch', 'layers', 'layers_clear', 'leak_add', 'leak_remove', 'lens', 'library_add', 'library_books', 'library_music',
            'lightbulb_outline', 'line_style', 'line_weight', 'linear_scale', 'link', 'linked_camera', 'list', 'live_help', 'live_tv', 'local_activity', 'local_airport',
            'local_atm', 'local_bar', 'local_cafe', 'local_car_wash', 'local_convenience_store', 'local_dining', 'local_drink', 'local_florist', 'local_gas_station',
            'local_grocery_store', 'local_hospital', 'local_hotel', 'local_laundry_service', 'local_library', 'local_mall', 'local_movies', 'local_offer', 'local_parking',
            'local_pharmacy', 'local_phone', 'local_pizza', 'local_play', 'local_post_office', 'local_printshop', 'local_see', 'local_shipping', 'local_taxi',
            'location_city', 'location_disabled', 'location_off', 'location_on', 'location_searching', 'lock', 'lock_open', 'lock_outline', 'looks', 'looks_3',
            'looks_4', 'looks_5', 'looks_6', 'looks_one', 'looks_two', 'loop', 'loupe', 'low_priority', 'loyalty', 'mail', 'mail_outline', 'map', 'markunread',
            'markunread_mailbox', 'memory', 'menu', 'merge_type', 'message', 'mic', 'mic_none', 'mic_off', 'mms', 'mode_comment', 'mode_edit', 'monetization_on',
            'money_off', 'monochrome_photos', 'mood', 'mood_bad', 'more', 'more_horiz', 'more_vert', 'motorcycle', 'mouse', 'move_to_inbox', 'movie', 'movie_creation',
            'movie_filter', 'multiline_chart', 'music_note', 'music_video', 'my_location', 'nature', 'nature_people', 'navigate_before', 'navigate_next', 'navigation',
            'near_me', 'network_cell', 'network_check', 'network_locked', 'network_wifi', 'new_releases', 'next_week', 'nfc', 'no_encryption', 'no_sim', 'not_interested',
            'note', 'note_add', 'notifications', 'notifications_active', 'notifications_none', 'notifications_off', 'notifications_paused', 'offline_pin', 'ondemand_video',
            'opacity', 'open_in_browser', 'open_in_new', 'open_with', 'pages', 'pageview', 'palette', 'pan_tool', 'panorama', 'panorama_fish_eye', 'panorama_horizontal',
            'panorama_vertical', 'panorama_wide_angle', 'party_mode', 'pause', 'pause_circle_filled', 'pause_circle_outline', 'payment', 'people', 'people_outline',
            'perm_camera_mic', 'perm_contact_calendar', 'perm_data_setting', 'perm_device_information', 'perm_identity', 'perm_media', 'perm_phone_msg', 'perm_scan_wifi',
            'person', 'person_add', 'person_outline', 'person_pin', 'person_pin_circle', 'personal_video', 'pets', 'phone', 'phone_android',
            'phone_bluetooth_speaker', 'phone_forwarded', 'phone_in_talk', 'phone_iphone', 'phone_locked', 'phone_missed', 'phone_paused',
            'phonelink', 'phonelink_erase', 'phonelink_lock', 'phonelink_off', 'phonelink_ring', 'phonelink_setup', 'photo', 'photo_album',
            'photo_camera', 'photo_filter', 'photo_library', 'photo_size_select_actual', 'photo_size_select_large', 'photo_size_select_small', 'picture_as_pdf',
            'picture_in_picture', 'picture_in_picture_alt', 'pie_chart', 'pie_chart_outlined', 'pin_drop', 'place', 'play_arrow', 'play_circle_filled',
            'play_circle_outline', 'play_for_work', 'playlist_add', 'playlist_add_check', 'playlist_play', 'plus_one', 'poll', 'polymer', 'pool', 'portable_wifi_off',
            'portrait', 'power', 'power_input', 'power_settings_new', 'pregnant_woman', 'present_to_all', 'print', 'priority_high', 'public', 'publish',
            'query_builder', 'question_answer', 'queue', 'queue_music', 'queue_play_next', 'radio', 'radio_button_checked', 'radio_button_unchecked', 'rate_review',
            'receipt', 'recent_actors', 'record_voice_over', 'redeem', 'redo', 'refresh', 'remove', 'remove_circle', 'remove_circle_outline', 'remove_from_queue',
            'remove_red_eye', 'remove_shopping_cart', 'reorder', 'repeat', 'repeat_one', 'replay', 'replay_10', 'replay_30', 'replay_5', 'reply', 'reply_all',
            'report', 'report_problem', 'restaurant', 'restaurant_menu', 'restore', 'restore_page', 'ring_volume', 'room', 'room_service', 'rotate_90_degrees_ccw',
            'rotate_left', 'rotate_right', 'rounded_corner', 'router', 'rowing', 'rss_feed', 'rv_hookup', 'satellite', 'save', 'scanner', 'schedule', 'school',
            'screen_lock_landscape', 'screen_lock_portrait', 'screen_lock_rotation', 'screen_rotation', 'screen_share', 'sd_card', 'sd_storage', 'search', 'security',
            'select_all', 'send', 'sentiment_dissatisfied', 'sentiment_neutral', 'sentiment_satisfied', 'sentiment_very_dissatisfied', 'sentiment_very_satisfied',
            'settings', 'settings_applications', 'settings_backup_restore', 'settings_bluetooth', 'settings_brightness', 'settings_cell', 'settings_ethernet',
            'settings_input_antenna', 'settings_input_component', 'settings_input_composite', 'settings_input_hdmi', 'settings_input_svideo', 'settings_overscan',
            'settings_phone', 'settings_power', 'settings_remote', 'settings_system_daydream', 'settings_voice', 'share', 'shop', 'shop_two', 'shopping_basket',
            'shopping_cart', 'short_text', 'show_chart', 'shuffle', 'signal_cellular_4_bar', 'signal_cellular_connected_no_internet_4_bar', 'signal_cellular_no_sim',
            'signal_cellular_null', 'signal_cellular_off', 'signal_wifi_4_bar', 'signal_wifi_4_bar_lock', 'signal_wifi_off', 'sim_card', 'sim_card_alert',
            'skip_next', 'skip_previous', 'slideshow', 'slow_motion_video', 'smartphone', 'smoke_free', 'smoking_rooms', 'sms', 'sms_failed', 'snooze',
            'sort', 'sort_by_alpha', 'spa', 'space_bar', 'speaker', 'speaker_group', 'speaker_notes', 'speaker_notes_off', 'speaker_phone', 'spellcheck', 'star',
            'star_border', 'star_half', 'stars', 'stay_current_landscape', 'stay_current_portrait', 'stay_primary_landscape', 'stay_primary_portrait',
            'stop', 'stop_screen_share', 'storage', 'store', 'store_mall_directory', 'straighten', 'streetview', 'strikethrough_s', 'style', 'subdirectory_arrow_left',
            'subdirectory_arrow_right', 'subject', 'subscriptions', 'subtitles', 'subway', 'supervisor_account', 'surround_sound', 'swap_calls', 'swap_horiz',
            'swap_vert', 'swap_vertical_circle', 'switch_camera', 'switch_video', 'sync', 'sync_disabled', 'sync_problem', 'system_update', 'system_update_alt',
            'tab', 'tab_unselected', 'tablet', 'tablet_android', 'tablet_mac', 'tag_faces', 'tap_and_play', 'terrain', 'text_fields', 'text_format', 'textsms',
            'texture', 'theaters', 'thumb_down', 'thumb_up', 'thumbs_up_down', 'time_to_leave', 'timelapse', 'timeline', 'timer', 'timer_10', 'timer_3', 'timer_off',
            'title', 'toc', 'today', 'toll', 'tonality', 'touch_app', 'toys', 'track_changes', 'traffic', 'train', 'tram', 'transfer_within_a_station',
            'transform', 'translate', 'trending_down', 'trending_flat', 'trending_up', 'tune', 'turned_in', 'turned_in_not', 'tv', 'unarchive', 'undo',
            'unfold_less', 'unfold_more', 'update', 'usb', 'verified_user', 'vertical_align_bottom', 'vertical_align_center', 'vertical_align_top',
            'vibration', 'video_call', 'video_label', 'video_library', 'videocam', 'videocam_off', 'videogame_asset', 'view_agenda', 'view_array', 'view_carousel',
            'view_column', 'view_comfy', 'view_compact', 'view_day', 'view_headline', 'view_list', 'view_module', 'view_quilt', 'view_stream', 'view_week', 'vignette',
            'visibility', 'visibility_off', 'voice_chat', 'voicemail', 'volume_down', 'volume_mute', 'volume_off', 'volume_up', 'vpn_key', 'vpn_lock', 'wallpaper',
            'warning', 'watch', 'watch_later', 'wb_auto', 'wb_cloudy', 'wb_incandescent', 'wb_iridescent', 'wb_sunny', 'wc', 'web', 'web_asset', 'weekend', 'whatshot',
            'widgets', 'wifi', 'wifi_lock', 'wifi_tethering', 'work', 'wrap_text', 'youtube_searched_for', 'zoom_in', 'zoom_out', 'zoom_out_map']
    }
}