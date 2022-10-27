import { atom } from "recoil";
import { UserModel } from "./Models";

export const localUserState = atom<UserModel | undefined>({
    key: 'LocalUserState',
    default: undefined
});
