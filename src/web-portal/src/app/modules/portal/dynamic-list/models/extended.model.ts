import { FilterField, ShellOption } from "services/portal.service";
import { Constants } from 'portal/resources/constants';
import { ExtendedShellOption } from 'portal/shared/shelloptions/extened.shell.model';
import * as _ from 'lodash';

export interface ExtendedFilterField extends FilterField {
    jsonData: any
    datasourceId: string
}

export class ListOptions {

    sizeOptions: number[]
    defaultPageSize: number
    fetchDataFirstTime: boolean
    maximumColumns: number
    enableSearch: boolean
    enableAdvancedSearch: boolean
    enablePagination: boolean

    public static getListOptions(options: ShellOption[]): ListOptions {
        return {
            sizeOptions: JSON.parse(_.find(options, opt => opt.key === 'sizeoptions').value),
            defaultPageSize: JSON.parse(_.find(options, opt => opt.key === 'defaultpagesize').value),
            fetchDataFirstTime: JSON.parse(_.find(options, opt => opt.key === 'fetchfirsttime').value),
            maximumColumns: JSON.parse(_.find(options, opt => opt.key === 'maximumcolumns').value),
            enableSearch: JSON.parse(_.find(options, opt => opt.key === 'enablesearch').value),
            enableAdvancedSearch: JSON.parse(_.find(options, opt => opt.key === 'enableadvancedsearch').value),
            enablePagination: JSON.parse(_.find(options, opt => opt.key === 'enablepagination').value)
        }
    }

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

    public static isAllowEdit(shell: ExtendedShellOption): boolean {
        return shell.key !== this.PageSizeOptions.key
            && shell.key !== this.DefaultPageSize.key
            && shell.key !== this.FetchDataFirstTime.key
            && shell.key !== this.MaximumColumns.key
            && shell.key !== this.EnableSearch.key
            && shell.key !== this.EnableAdvancedSearch.key
            && shell.key !== this.EnablePagination.key
    }

    public static getDefaultShellOptionsForList(): ExtendedShellOption[] {
        return [
            this.PageSizeOptions,
            this.PageSizeOptions,
            this.FetchDataFirstTime,
            this.MaximumColumns,
            this.EnableSearch,
            this.EnableAdvancedSearch,
            this.EnablePagination
        ]
    }

    public static getPageSizeOptions(optValue: string) {
        return JSON.parse(optValue)
    }

    public static DefaultListOptions: ListOptions =  {
        defaultPageSize: 10,
        enableAdvancedSearch: true,
        enablePagination: true,
        enableSearch: true,
        fetchDataFirstTime: true,
        maximumColumns: 6,
        sizeOptions: [5, 10, 20, 30, 50]
    }
}