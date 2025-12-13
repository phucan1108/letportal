import { ObjectUtils } from 'app/core/utils/object-util';
import { ExtendedShellOption } from 'portal/shared/shelloptions/extened.shell.model';
import { FilterField, ShellOption } from 'services/portal.service';
 

export interface ExtendedFilterField extends FilterField {
    jsonData: any
    datasourceId: string
}

export class ListOptions {

    public static PageSizeOptions: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Number of items will be displayed. Default: 5,10,20,50',
        key: 'sizeoptions',
        value: '[ 5, 10, 20, 50 ]'
    }

    public static DefaultPageSize: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'The default number of items. Default: 10',
        key: 'defaultpagesize',
        value: '10'
    }

    public static FetchDataFirstTime: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Allow calling the data when list is appeared in page. Default: true',
        key: 'fetchfirsttime',
        value: 'true'
    }

    public static MaximumColumns: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'When a number of columns is over this value, Details button will be displayed. Default: 6',
        key: 'maximumcolumns',
        value: '6'
    }

    public static EnableSearch: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'If it is false, so a search textbox will be disappeared. Default: true',
        key: 'enablesearch',
        value: 'true'
    }

    public static EnableAdvancedSearch: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'If it is false, so an advanced search will be disappeared. Default: true',
        key: 'enableadvancedsearch',
        value: 'true'
    }

    public static EnablePagination: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'If it is false, so a pagination will be disappeared. Default: true',
        key: 'enablepagination',
        value: 'true'
    }

    public static EnableExportExcel: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'If it is false, so an export button will be disappeared. Default: true',
        key: 'enableexportexcel',
        value: 'true'
    }

    public static MaximumClientExport: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Maximum records of exporting excel on client side. If a total is over this number, we will use server-side. Default: 100',
        key: 'maximumclientexport',
        value: '200'
    }

    public static AllowExportHiddenFields: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'If it is true, user can export hidden fields. Default: false',
        key: 'allowexporthiddenfields',
        value: 'false'
    }

    public static DefaultSortCol: ExtendedShellOption = {
        id: '',
        allowDelete: false,
        description: 'Enter the index of default sort column and sort direction(asc,desc). 0 means the system will automatically choose one column. Count from 1. Default: 0|asc',
        key: 'defaultsortcol',
        value: '0|asc'
    }

    public static DefaultListOptions: ListOptions =  {
        defaultPageSize: 10,
        enableAdvancedSearch: true,
        enablePagination: true,
        enableSearch: true,
        fetchDataFirstTime: true,
        maximumColumns: 6,
        sizeOptions: [5, 10, 20, 30, 50],
        enableExportExcel: true,
        maximumClientExport: 1000,
        allowExportHiddenFields: false,
        defaultSortCol: '0|asc'
    }

    sizeOptions: number[]
    defaultPageSize: number
    fetchDataFirstTime: boolean
    maximumColumns: number
    enableSearch: boolean
    enableAdvancedSearch: boolean
    enablePagination: boolean
    enableExportExcel: boolean
    maximumClientExport: number
    allowExportHiddenFields: boolean
    defaultSortCol: string

    public static getListOptions(options: ShellOption[]): ListOptions {
        return {
            sizeOptions: JSON.parse(options.find(opt => opt.key === 'sizeoptions').value),
            defaultPageSize: JSON.parse(options.find(opt => opt.key === 'defaultpagesize').value),
            fetchDataFirstTime: JSON.parse(options.find(opt => opt.key === 'fetchfirsttime').value),
            maximumColumns: JSON.parse(options.find(opt => opt.key === 'maximumcolumns').value),
            enableSearch: JSON.parse(options.find(opt => opt.key === 'enablesearch').value),
            enableAdvancedSearch: JSON.parse(options.find(opt => opt.key === 'enableadvancedsearch').value),
            enablePagination: JSON.parse(options.find(opt => opt.key === 'enablepagination').value),
            enableExportExcel: JSON.parse(options.find(opt => opt.key === 'enableexportexcel').value),
            maximumClientExport: JSON.parse(options.find(opt => opt.key === 'maximumclientexport').value)            ,
            allowExportHiddenFields: JSON.parse(options.find(opt => opt.key === 'allowexporthiddenfields').value),
            defaultSortCol: options.find(opt => opt.key === 'defaultsortcol') ? options.find(opt => opt.key === 'defaultsortcol').value : '0|asc'
        }
    }

    public static isAllowEdit(shell: ExtendedShellOption): boolean {
        return shell.key !== this.PageSizeOptions.key
            && shell.key !== this.DefaultPageSize.key
            && shell.key !== this.FetchDataFirstTime.key
            && shell.key !== this.MaximumColumns.key
            && shell.key !== this.EnableSearch.key
            && shell.key !== this.EnableAdvancedSearch.key
            && shell.key !== this.EnablePagination.key
            && shell.key !== this.EnableExportExcel.key
            && shell.key !== this.MaximumClientExport.key
            && shell.key !== this.AllowExportHiddenFields.key
            && shell.key !== this.DefaultSortCol.key
    }

    public static getDefaultShellOptionsForList(): ExtendedShellOption[] {
        return [
            this.PageSizeOptions,
            this.DefaultPageSize,
            this.FetchDataFirstTime,
            this.MaximumColumns,
            this.EnableSearch,
            this.EnableAdvancedSearch,
            this.EnablePagination,
            this.EnableExportExcel,
            this.MaximumClientExport,
            this.AllowExportHiddenFields,
            this.DefaultSortCol
        ]
    }

    public static combinedDefaultShellOptions(opts: ExtendedShellOption[]){
        const defaultOpts = this.getDefaultShellOptionsForList()
        if(!ObjectUtils.isNotNull(opts)){
            opts = defaultOpts
        }
        let nonFoundOpts: ExtendedFilterField[] = []
        defaultOpts.forEach(a => {
            const found = opts.find(b => b.key === a.key)
            if(found){
                found.allowDelete = a.allowDelete
                found.description = a.description
            }
            else{
                opts.push(ObjectUtils.clone(a))
            }
        })
    }

    public static getPageSizeOptions(optValue: string) {
        return JSON.parse(optValue)
    }
}