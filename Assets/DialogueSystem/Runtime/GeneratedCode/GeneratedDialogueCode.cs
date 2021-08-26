using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using DialogueSystem;

namespace DialogueSystem {
public class GeneratedDialogueCode{
public delegate void eventDelegate();
public delegate bool conditionDelegate();
private Dictionary<string, eventDelegate> eventFunctions = new Dictionary<string, eventDelegate>();
private Dictionary<string, conditionDelegate> conditionChecks = new Dictionary<string, conditionDelegate>();
private Dictionary<string, conditionDelegate> dialogueChecks = new Dictionary<string, conditionDelegate>();
public Dictionary<string, eventDelegate> GetEventFunctions() { return eventFunctions; }
public Dictionary<string, conditionDelegate> GetConditionChecks() { return conditionChecks; }
public Dictionary<string, conditionDelegate> GetDialogueChecks() { return dialogueChecks; }




// Container: New Dialogue //


// Node: 38013ccc-4839-4be0-bec0-505a0c69e8dd //
int fun = 0;


public void Start() {
eventFunctions.Add("New_Dialogue_1eb83f9581a34f4ca59603054104c516",New_Dialogue_1eb83f9581a34f4ca59603054104c516);
eventFunctions.Add("New_Dialogue_498f128a0a1544d39eefa5194b6d058d",New_Dialogue_498f128a0a1544d39eefa5194b6d058d);
conditionChecks.Add("New_Dialogue_149e58defaa240fdabd4ac1f2f60327c",New_Dialogue_149e58defaa240fdabd4ac1f2f60327c);
conditionChecks.Add("New_Dialogue_e7fb90dd018147a2885d49cbce973e60",New_Dialogue_e7fb90dd018147a2885d49cbce973e60);
dialogueChecks.Add("New_Dialogue_170a9adb3f0447159a1b47da17e0de11_b980229a3b0f43c986822b401f261933",New_Dialogue_170a9adb3f0447159a1b47da17e0de11_b980229a3b0f43c986822b401f261933);
dialogueChecks.Add("New_Dialogue_170a9adb3f0447159a1b47da17e0de11_fb857a76b1c04497948368bc0a5e972e",New_Dialogue_170a9adb3f0447159a1b47da17e0de11_fb857a76b1c04497948368bc0a5e972e);
dialogueChecks.Add("New_Dialogue_170a9adb3f0447159a1b47da17e0de11_8ac4517828a049afa6abc23a54625ed3",New_Dialogue_170a9adb3f0447159a1b47da17e0de11_8ac4517828a049afa6abc23a54625ed3);

}



// Container: New Dialogue //


// Node: 170a9adb-3f04-4715-9a1b-47da17e0de11 //
// Choice: b980229a-3b0f-43c9-8682-2b401f261933 //
public bool New_Dialogue_170a9adb3f0447159a1b47da17e0de11_b980229a3b0f43c986822b401f261933() {
return (fun == 1);
}


// Node: 170a9adb-3f04-4715-9a1b-47da17e0de11 //
// Choice: fb857a76-b1c0-4497-9483-68bc0a5e972e //
public bool New_Dialogue_170a9adb3f0447159a1b47da17e0de11_fb857a76b1c04497948368bc0a5e972e() {
return (fun == 2);
}


// Node: 170a9adb-3f04-4715-9a1b-47da17e0de11 //
// Choice: 8ac45178-28a0-49af-a6ab-c23a54625ed3 //
public bool New_Dialogue_170a9adb3f0447159a1b47da17e0de11_8ac4517828a049afa6abc23a54625ed3() {
return (fun == 3);
}





// Container: New Dialogue //


// Node: 149e58de-faa2-40fd-abd4-ac1f2f60327c //
public bool New_Dialogue_149e58defaa240fdabd4ac1f2f60327c() {
return (true);
}


// Node: e7fb90dd-0181-47a2-885d-49cbce973e60 //
public bool New_Dialogue_e7fb90dd018147a2885d49cbce973e60() {
return (fun <= 5);
}





// Container: New Dialogue //


// Node: 1eb83f95-81a3-4f4c-a596-03054104c516 //
public void New_Dialogue_1eb83f9581a34f4ca59603054104c516() {
// TODO //
}


// Node: 498f128a-0a15-44d3-9eef-a5194b6d058d //
public void New_Dialogue_498f128a0a1544d39eefa5194b6d058d() {
fun = 1000;
}




}
}
