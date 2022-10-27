import { atom } from "recoil";
import { UserModel } from "./Models";

const defaultUser: UserModel = {
    id: 1,
    username: "Laco",
    messages: [],
    conversations: []
}

export const currentUserState = atom({
    key: 'UserIdState',
    default: defaultUser
});
