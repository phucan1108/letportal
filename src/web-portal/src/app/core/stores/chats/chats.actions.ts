import { ClickedOnChatUserEvent, TakeUserOnlineEvent, ActiveDoubleChatActionEvent, OpenNewDoubleChatRoomEvent as FetchedNewDoubleChatRoomEvent, SendNewMessageEvent, ReceivedMessageEvent, ClickedOnChatBoxIconEvent, LoadingMoreSessionEvent, LoadedMoreSessionEvent, AddedNewSessionEvent, ReceivedMessageFromAnotherDeviceEvent, InitialReceivedChatRoomEvent, NotifyNewIncomingMessageEvent, LoadedAllAvailableUsersEvent, IncomingOnlineUserEvent, IncomingOfflineUserEvent, IncomingMessageFromUnloadUserEvent, IncomingVideoCallEvent, HandshakedVideoCallEvent, ReceivedIceServerEvent } from './chats.events'
import { ErrorCode } from 'services/identity.service'

const CHAT_ACTION = '[Chat Action]'

export class ChatAction {
    public static readonly type = ''
}

export class TakeUserOnline implements ChatAction {
    public static readonly type = `${CHAT_ACTION} User Online`
    constructor(public event: TakeUserOnlineEvent) { }
}

export class GotHubChatProblem implements ChatAction {
    public static readonly type = `${CHAT_ACTION} Got Hub Chat Problem`
    constructor() { }
}
export class ActiveChatSearchBox implements ChatAction {
    public static readonly type = `${CHAT_ACTION} Active Chat Search`
    constructor() { }
}

export class ActiveDoubleChatRoom implements ChatAction {
    public static readonly type = `${CHAT_ACTION} Active Double Chat Room`
    constructor(public event: ActiveDoubleChatActionEvent) { }
}

export class LoadingMoreSession implements ChatAction{
    public static readonly type = `${CHAT_ACTION} Loading more session`
    constructor() { }
}

export class LoadedMoreSession implements ChatAction {
    public static readonly type = `${CHAT_ACTION} Loaded more session`
    constructor(public event: LoadedMoreSessionEvent) { }
}

export class AddedNewSession implements ChatAction{
    public static readonly type = `${CHAT_ACTION} Added new session`
    constructor(public event: AddedNewSessionEvent) { }
}

export class FetchDoubleChatRoom implements ChatAction {
    public static readonly type = `${CHAT_ACTION} Fetch Double Chat Room`
    constructor() { }
}
export class FetchedNewDoubleChatRoom implements ChatAction {
    public static readonly type = `${CHAT_ACTION} Fetched new double Chat Room`
    constructor(public event: FetchedNewDoubleChatRoomEvent) { }
}

export class ClickedOnChatUser implements ChatAction {
    public static readonly type = `${CHAT_ACTION} User clicked on chat user in search box`
    constructor(public event: ClickedOnChatUserEvent) { }
}

export class ClickedOnChatBox implements ChatAction {
    public static readonly type = `${CHAT_ACTION} User clicked on chat box icon`
    constructor(public event: ClickedOnChatBoxIconEvent) { }
}

export class InitialReceivedChatRoom implements ChatAction{
    public static readonly type = `${CHAT_ACTION} User has received unload chat room`
    constructor(public event: InitialReceivedChatRoomEvent){ }
}

export class NotifyNewIncomingMessage implements ChatAction{
    public static readonly type = `${CHAT_ACTION} User has received new incoming message`
    constructor(public event: NotifyNewIncomingMessageEvent) { }
}

export class SentMessage implements ChatAction {
    public static readonly type = `${CHAT_ACTION} User sent message`
    constructor(public event: SendNewMessageEvent) { }
}

export class ReceivedMessage implements ChatAction {
    public static readonly type = `${CHAT_ACTION} User received message`
    constructor(public event: ReceivedMessageEvent) { }
}

export class ReceivedMessageFromAnotherDevice implements ChatAction{
    public static readonly type = `${CHAT_ACTION} User received message from another device`
    constructor(public event: ReceivedMessageFromAnotherDeviceEvent) { }
}

export class ToggleOpenChatRoom implements ChatAction{
    public static readonly type = `${CHAT_ACTION} Toggle open chat room`
    constructor(public toggle: boolean) { }
}

export class LoadedAllAvailableUsers implements ChatAction {
    public static readonly type = `${CHAT_ACTION} Loaded all available users`
    constructor(public event: LoadedAllAvailableUsersEvent) { }
}

export class IncomingOnlineUser implements ChatAction {
    public static readonly type = `${CHAT_ACTION} Incoming online user`
    constructor(public event: IncomingOnlineUserEvent) { }
}

export class IncomingOfflineUser implements ChatAction {
    public static readonly type = `${CHAT_ACTION} Incoming offline user`
    constructor(public event: IncomingOfflineUserEvent) { }
}

export class IncomingMessageFromUnloadUser implements ChatAction {
    public static readonly type = `${CHAT_ACTION} Incoming message unload user`
    constructor(public event: IncomingMessageFromUnloadUserEvent) { }
}

export class RemoveLastLongActiveChatRoom implements ChatAction {
    public static readonly type = `${CHAT_ACTION} Remove last long active chat room`
    constructor(public newChatRoomId: string) { }
}

export class NotifyIncomingVideoCall implements ChatAction {
    public static readonly type = `${CHAT_ACTION} User has upcoming video call`
    constructor(public event: IncomingVideoCallEvent) { }
}

export class HandshakedVideoCall implements ChatAction {
    public static readonly type = `${CHAT_ACTION} User has handshaked video call`
    constructor(public event: HandshakedVideoCallEvent) { }
}

export class ReceivedIceServer implements ChatAction {
    public static readonly type = `${CHAT_ACTION} Received ICE Server`
    constructor(public event: ReceivedIceServerEvent) { }
}

export class DroppedCall implements ChatAction {
    public static readonly type = `${CHAT_ACTION} User dropped a video call`
    constructor() { }
}

export class UserCancelledCall implements ChatAction {
    public static readonly type = `${CHAT_ACTION} Caller cancelled a video call`
    constructor() { }
}

export class UserDroppedCall implements ChatAction {
    public static readonly type = `${CHAT_ACTION} Partner dropped a video call`
    constructor() { }
}

export class ForceDroppedCall implements ChatAction {
    public static readonly type = `${CHAT_ACTION} Forced droppped a video call`
    constructor(public error: ErrorCode) { }
}

export class UserDeniedCall implements ChatAction {
    public static readonly type = `${CHAT_ACTION} Partner denied a video call`
    constructor() { }
}

export type All =
    ChatAction |
    TakeUserOnline |
    ActiveChatSearchBox |
    ActiveDoubleChatRoom |
    ClickedOnChatUser |
    FetchDoubleChatRoom |
    FetchedNewDoubleChatRoom |
    SentMessage |
    ReceivedMessage |
    LoadedAllAvailableUsers| 
    IncomingOnlineUser |
    IncomingOfflineUser |
    IncomingMessageFromUnloadUser