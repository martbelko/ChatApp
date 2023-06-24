export interface UserModel
{
	id: number;
	username: string;
	messages?: Array<MessageModel>;
	conversations?: Array<ConversationModel>;
}

export interface MessageModel
{
	id: number;
	content: string;
	datetime: Date;
	author: UserModel;
	conversation?: ConversationModel;
}

export interface ConversationModel
{
	id: number;
	members: Array<UserModel>;
	messages: Array<MessageModel>;
}
