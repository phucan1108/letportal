import { FilterOption, FilterField, ColumnDef } from 'services/portal.service';


export interface ExtendedFilterOption extends FilterOption {
    filterDataSource: Array<any>,
    filterOperators: Array<any>
}

export interface ExtendedRenderFilterField extends FilterField {
    jsonData: any
    datasourceId: string
}

export interface ExtendedColDef extends ColumnDef{
    datasourceId: string
    inDetailMode: boolean
}