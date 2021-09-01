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
public string GetVariable(string variableName) { return this.GetType().GetField(variableName).GetValue(this).ToString(); }




// Container: New Dialogue //


// Node: 38013ccc-4839-4be0-bec0-505a0c69e8dd //
int fun = 0;


// Container: test //


// Node: 361b13b6-7e09-4a3f-a909-e907b9484546 //
int age;

// Node: 8d313b4e-4ad3-46cc-922c-343df5237c4d //
string testingColor = "";


public void Start() {
eventFunctions.Add("New_Dialogue_1eb83f9581a34f4ca59603054104c516",New_Dialogue_1eb83f9581a34f4ca59603054104c516);
eventFunctions.Add("New_Dialogue_498f128a0a1544d39eefa5194b6d058d",New_Dialogue_498f128a0a1544d39eefa5194b6d058d);
conditionChecks.Add("New_Dialogue_e7fb90dd018147a2885d49cbce973e60",New_Dialogue_e7fb90dd018147a2885d49cbce973e60);
conditionChecks.Add("New_Dialogue_861ad73dcb4b4b88bb45648115de3356",New_Dialogue_861ad73dcb4b4b88bb45648115de3356);
conditionChecks.Add("New_Dialogue_149e58defaa240fdabd4ac1f2f60327c",New_Dialogue_149e58defaa240fdabd4ac1f2f60327c);
dialogueChecks.Add("New_Dialogue_170a9adb3f0447159a1b47da17e0de11_a66d27517b5146f688619a24bfdda113",New_Dialogue_170a9adb3f0447159a1b47da17e0de11_a66d27517b5146f688619a24bfdda113);
dialogueChecks.Add("New_Dialogue_170a9adb3f0447159a1b47da17e0de11_ae8d8931fd1c488b85b4acc549975dfa",New_Dialogue_170a9adb3f0447159a1b47da17e0de11_ae8d8931fd1c488b85b4acc549975dfa);
dialogueChecks.Add("New_Dialogue_170a9adb3f0447159a1b47da17e0de11_b0b90bb438f34f479c2c5203d1ca477b",New_Dialogue_170a9adb3f0447159a1b47da17e0de11_b0b90bb438f34f479c2c5203d1ca477b);
eventFunctions.Add("test_7cb18f903e354491b4c5bb49df7c0b97",test_7cb18f903e354491b4c5bb49df7c0b97);
eventFunctions.Add("test_3c7202c457194aba90534087dad1f847",test_3c7202c457194aba90534087dad1f847);
eventFunctions.Add("test_db8a2717fef647e88b41094e49d38e07",test_db8a2717fef647e88b41094e49d38e07);
eventFunctions.Add("test_404f9463894a450395643e008367682e",test_404f9463894a450395643e008367682e);
eventFunctions.Add("test_428cede54991402283e74ba2b37ce117",test_428cede54991402283e74ba2b37ce117);
eventFunctions.Add("test_0286d59b26d64e48b3fe4e73229e9b1b",test_0286d59b26d64e48b3fe4e73229e9b1b);
eventFunctions.Add("test_8a99f93694bb426386eb40c8de7c5297",test_8a99f93694bb426386eb40c8de7c5297);
conditionChecks.Add("test_c2a8a7f56bcd43b9b508abcd5e4bc8e2",test_c2a8a7f56bcd43b9b508abcd5e4bc8e2);
conditionChecks.Add("test_94dfa2793b564602b69ac48debe0e80d",test_94dfa2793b564602b69ac48debe0e80d);
dialogueChecks.Add("test_c0ca3835e1bc446c99c96184241c1447_8ca9394ff37b44c398bf243ac7be493e",test_c0ca3835e1bc446c99c96184241c1447_8ca9394ff37b44c398bf243ac7be493e);
dialogueChecks.Add("test_c0ca3835e1bc446c99c96184241c1447_0ca3f6c6554049cc91b9204e5dd9250c",test_c0ca3835e1bc446c99c96184241c1447_0ca3f6c6554049cc91b9204e5dd9250c);
dialogueChecks.Add("test_c0ca3835e1bc446c99c96184241c1447_8519f8cccd27431f81b4223e8fe7a6a5",test_c0ca3835e1bc446c99c96184241c1447_8519f8cccd27431f81b4223e8fe7a6a5);
dialogueChecks.Add("test_c0ca3835e1bc446c99c96184241c1447_2af6e5bad1d34805b0558b7a6f4a12ca",test_c0ca3835e1bc446c99c96184241c1447_2af6e5bad1d34805b0558b7a6f4a12ca);
dialogueChecks.Add("test_ce8a9c528cca490d9512f5764eb8668a_9c36806ec69542c085a1c2421c0c4011",test_ce8a9c528cca490d9512f5764eb8668a_9c36806ec69542c085a1c2421c0c4011);
dialogueChecks.Add("test_ce8a9c528cca490d9512f5764eb8668a_f7b8de4e4cf2470289ac46299bb2ff42",test_ce8a9c528cca490d9512f5764eb8668a_f7b8de4e4cf2470289ac46299bb2ff42);
dialogueChecks.Add("test_ce8a9c528cca490d9512f5764eb8668a_25136c7a5b0946e18e8216653ab9e5ab",test_ce8a9c528cca490d9512f5764eb8668a_25136c7a5b0946e18e8216653ab9e5ab);
dialogueChecks.Add("test_ce8a9c528cca490d9512f5764eb8668a_5e5383a350e34b808a3596c63bf8e0c3",test_ce8a9c528cca490d9512f5764eb8668a_5e5383a350e34b808a3596c63bf8e0c3);
dialogueChecks.Add("test_285220cc140648efbb5b3efb33248ded_22e36fdf94c341b892a817119e887eca",test_285220cc140648efbb5b3efb33248ded_22e36fdf94c341b892a817119e887eca);
dialogueChecks.Add("test_285220cc140648efbb5b3efb33248ded_1d6a4e3792dc46d7a8112fb76c06b1b2",test_285220cc140648efbb5b3efb33248ded_1d6a4e3792dc46d7a8112fb76c06b1b2);
dialogueChecks.Add("test_938cb1b6e820464f808ecc7ee2d36be2_6d095fe2fe8e411e977c81fec197ce6a",test_938cb1b6e820464f808ecc7ee2d36be2_6d095fe2fe8e411e977c81fec197ce6a);
dialogueChecks.Add("test_938cb1b6e820464f808ecc7ee2d36be2_f13bd8a4671a442a8edf4f68b0cf305b",test_938cb1b6e820464f808ecc7ee2d36be2_f13bd8a4671a442a8edf4f68b0cf305b);
dialogueChecks.Add("test_938cb1b6e820464f808ecc7ee2d36be2_f376aea7020947198d4758258467fc71",test_938cb1b6e820464f808ecc7ee2d36be2_f376aea7020947198d4758258467fc71);
dialogueChecks.Add("test_938cb1b6e820464f808ecc7ee2d36be2_fb6097e749234cb48f9d305e662c3315",test_938cb1b6e820464f808ecc7ee2d36be2_fb6097e749234cb48f9d305e662c3315);
dialogueChecks.Add("test_938cb1b6e820464f808ecc7ee2d36be2_befe7ed1c70948fb84062920a25c2594",test_938cb1b6e820464f808ecc7ee2d36be2_befe7ed1c70948fb84062920a25c2594);
dialogueChecks.Add("test_938cb1b6e820464f808ecc7ee2d36be2_af70200f87cc4d7294b134ae88f3e08b",test_938cb1b6e820464f808ecc7ee2d36be2_af70200f87cc4d7294b134ae88f3e08b);
dialogueChecks.Add("test_938cb1b6e820464f808ecc7ee2d36be2_da995ddc00f1413eaf5b080b302e2d6b",test_938cb1b6e820464f808ecc7ee2d36be2_da995ddc00f1413eaf5b080b302e2d6b);
dialogueChecks.Add("test_938cb1b6e820464f808ecc7ee2d36be2_f55a19ffd4004df3b7deb492d4bb290f",test_938cb1b6e820464f808ecc7ee2d36be2_f55a19ffd4004df3b7deb492d4bb290f);

}



// Container: New Dialogue //


// Node: 170a9adb-3f04-4715-9a1b-47da17e0de11 //
// Choice: a66d2751-7b51-46f6-8861-9a24bfdda113 //
public bool New_Dialogue_170a9adb3f0447159a1b47da17e0de11_a66d27517b5146f688619a24bfdda113() {
return (fun == 3);
}


// Node: 170a9adb-3f04-4715-9a1b-47da17e0de11 //
// Choice: ae8d8931-fd1c-488b-85b4-acc549975dfa //
public bool New_Dialogue_170a9adb3f0447159a1b47da17e0de11_ae8d8931fd1c488b85b4acc549975dfa() {
return (fun == 2);
}


// Node: 170a9adb-3f04-4715-9a1b-47da17e0de11 //
// Choice: b0b90bb4-38f3-4f47-9c2c-5203d1ca477b //
public bool New_Dialogue_170a9adb3f0447159a1b47da17e0de11_b0b90bb438f34f479c2c5203d1ca477b() {
return (fun == 1);
}



// Container: test //


// Node: c0ca3835-e1bc-446c-99c9-6184241c1447 //
// Choice: 8ca9394f-f37b-44c3-98bf-243ac7be493e //
public bool test_c0ca3835e1bc446c99c96184241c1447_8ca9394ff37b44c398bf243ac7be493e() {
return (true);
}


// Node: c0ca3835-e1bc-446c-99c9-6184241c1447 //
// Choice: 0ca3f6c6-5540-49cc-91b9-204e5dd9250c //
public bool test_c0ca3835e1bc446c99c96184241c1447_0ca3f6c6554049cc91b9204e5dd9250c() {
return (true);
}


// Node: c0ca3835-e1bc-446c-99c9-6184241c1447 //
// Choice: 8519f8cc-cd27-431f-81b4-223e8fe7a6a5 //
public bool test_c0ca3835e1bc446c99c96184241c1447_8519f8cccd27431f81b4223e8fe7a6a5() {
return (true);
}


// Node: c0ca3835-e1bc-446c-99c9-6184241c1447 //
// Choice: 2af6e5ba-d1d3-4805-b055-8b7a6f4a12ca //
public bool test_c0ca3835e1bc446c99c96184241c1447_2af6e5bad1d34805b0558b7a6f4a12ca() {
return (true);
}


// Node: ce8a9c52-8cca-490d-9512-f5764eb8668a //
// Choice: 9c36806e-c695-42c0-85a1-c2421c0c4011 //
public bool test_ce8a9c528cca490d9512f5764eb8668a_9c36806ec69542c085a1c2421c0c4011() {
return (true);
}


// Node: ce8a9c52-8cca-490d-9512-f5764eb8668a //
// Choice: f7b8de4e-4cf2-4702-89ac-46299bb2ff42 //
public bool test_ce8a9c528cca490d9512f5764eb8668a_f7b8de4e4cf2470289ac46299bb2ff42() {
return (true);
}


// Node: ce8a9c52-8cca-490d-9512-f5764eb8668a //
// Choice: 25136c7a-5b09-46e1-8e82-16653ab9e5ab //
public bool test_ce8a9c528cca490d9512f5764eb8668a_25136c7a5b0946e18e8216653ab9e5ab() {
return (true);
}


// Node: ce8a9c52-8cca-490d-9512-f5764eb8668a //
// Choice: 5e5383a3-50e3-4b80-8a35-96c63bf8e0c3 //
public bool test_ce8a9c528cca490d9512f5764eb8668a_5e5383a350e34b808a3596c63bf8e0c3() {
return (true);
}


// Node: 285220cc-1406-48ef-bb5b-3efb33248ded //
// Choice: 22e36fdf-94c3-41b8-92a8-17119e887eca //
public bool test_285220cc140648efbb5b3efb33248ded_22e36fdf94c341b892a817119e887eca() {
return (true);
}


// Node: 285220cc-1406-48ef-bb5b-3efb33248ded //
// Choice: 1d6a4e37-92dc-46d7-a811-2fb76c06b1b2 //
public bool test_285220cc140648efbb5b3efb33248ded_1d6a4e3792dc46d7a8112fb76c06b1b2() {
return (true);
}


// Node: 938cb1b6-e820-464f-808e-cc7ee2d36be2 //
// Choice: 6d095fe2-fe8e-411e-977c-81fec197ce6a //
public bool test_938cb1b6e820464f808ecc7ee2d36be2_6d095fe2fe8e411e977c81fec197ce6a() {
return (age >= 35);
}


// Node: 938cb1b6-e820-464f-808e-cc7ee2d36be2 //
// Choice: f13bd8a4-671a-442a-8edf-4f68b0cf305b //
public bool test_938cb1b6e820464f808ecc7ee2d36be2_f13bd8a4671a442a8edf4f68b0cf305b() {
return (age <= 34 && age >= 18);
}


// Node: 938cb1b6-e820-464f-808e-cc7ee2d36be2 //
// Choice: f376aea7-0209-4719-8d47-58258467fc71 //
public bool test_938cb1b6e820464f808ecc7ee2d36be2_f376aea7020947198d4758258467fc71() {
return (age <= 17 && age >= 13);
}


// Node: 938cb1b6-e820-464f-808e-cc7ee2d36be2 //
// Choice: fb6097e7-4923-4cb4-8f9d-305e662c3315 //
public bool test_938cb1b6e820464f808ecc7ee2d36be2_fb6097e749234cb48f9d305e662c3315() {
return (age <= 12);
}


// Node: 938cb1b6-e820-464f-808e-cc7ee2d36be2 //
// Choice: befe7ed1-c709-48fb-8406-2920a25c2594 //
public bool test_938cb1b6e820464f808ecc7ee2d36be2_befe7ed1c70948fb84062920a25c2594() {
return (testingColor == "Yellow");
}


// Node: 938cb1b6-e820-464f-808e-cc7ee2d36be2 //
// Choice: af70200f-87cc-4d72-94b1-34ae88f3e08b //
public bool test_938cb1b6e820464f808ecc7ee2d36be2_af70200f87cc4d7294b134ae88f3e08b() {
return (testingColor == "Blue");
}


// Node: 938cb1b6-e820-464f-808e-cc7ee2d36be2 //
// Choice: da995ddc-00f1-413e-af5b-080b302e2d6b //
public bool test_938cb1b6e820464f808ecc7ee2d36be2_da995ddc00f1413eaf5b080b302e2d6b() {
return (testingColor == "Green");
}


// Node: 938cb1b6-e820-464f-808e-cc7ee2d36be2 //
// Choice: f55a19ff-d400-4df3-b7de-b492d4bb290f //
public bool test_938cb1b6e820464f808ecc7ee2d36be2_f55a19ffd4004df3b7deb492d4bb290f() {
return (testingColor == "Red");
}





// Container: New Dialogue //


// Node: e7fb90dd-0181-47a2-885d-49cbce973e60 //
public bool New_Dialogue_e7fb90dd018147a2885d49cbce973e60() {
return (fun <= 5);
}


// Node: 861ad73d-cb4b-4b88-bb45-648115de3356 //
public bool New_Dialogue_861ad73dcb4b4b88bb45648115de3356() {
return (true);
}


// Node: 149e58de-faa2-40fd-abd4-ac1f2f60327c //
public bool New_Dialogue_149e58defaa240fdabd4ac1f2f60327c() {
return (true);
}



// Container: test //


// Node: c2a8a7f5-6bcd-43b9-b508-abcd5e4bc8e2 //
public bool test_c2a8a7f56bcd43b9b508abcd5e4bc8e2() {
return (age > 12);
}


// Node: 94dfa279-3b56-4602-b69a-c48debe0e80d //
public bool test_94dfa2793b564602b69ac48debe0e80d() {
return (age > 34);
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



// Container: test //


// Node: 7cb18f90-3e35-4491-b4c5-bb49df7c0b97 //
public void test_7cb18f903e354491b4c5bb49df7c0b97() {
testingColor = "Red";
}


// Node: 3c7202c4-5719-4aba-9053-4087dad1f847 //
public void test_3c7202c457194aba90534087dad1f847() {
testingColor = "Yellow";
}


// Node: db8a2717-fef6-47e8-8b41-094e49d38e07 //
public void test_db8a2717fef647e88b41094e49d38e07() {
testingColor = "Green";
}


// Node: 404f9463-894a-4503-9564-3e008367682e //
public void test_404f9463894a450395643e008367682e() {
age = 0;
}


// Node: 428cede5-4991-4022-83e7-4ba2b37ce117 //
public void test_428cede54991402283e74ba2b37ce117() {
age = 13;
}


// Node: 0286d59b-26d6-4e48-b3fe-4e73229e9b1b //
public void test_0286d59b26d64e48b3fe4e73229e9b1b() {
age = 18;
}


// Node: 8a99f936-94bb-4263-86eb-40c8de7c5297 //
public void test_8a99f93694bb426386eb40c8de7c5297() {
age = 35;
}




}
}
