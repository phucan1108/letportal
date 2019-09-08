import { FilterOption, FilterField, ColumndDef } from 'services/portal.service';


export interface ExtendedFilterOption extends FilterOption {
    filterDataSource: Array<any>,
    filterOperators: Array<any>
}

export interface ExtendedRenderFilterField extends FilterField {
    jsonData: any
    datasourceId: string
}

export interface ExtendedColDef extends ColumndDef{
    datasourceId: string
    inDetailMode: boolean
}