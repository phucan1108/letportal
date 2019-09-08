import { CommandButtonInList } from 'services/portal.service';

export interface CommandClicked{
    command: CommandButtonInList,
    data: any
}

export interface DatasourceCache{
    datasourceId: string,
    data: any
}