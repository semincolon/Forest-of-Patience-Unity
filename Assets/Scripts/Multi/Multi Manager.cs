using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiManager : MonoBehaviourPunCallbacks
{
    private string gmaeVersion = "1.0";
    public TextMeshProUGUI connectionStateTxt;
    public Button joinBtn, enterBtn;
    public TMP_InputField mNickNameField, roomNumberField;
    public GameObject roomNumberUI;

    void Start()
    {
        PhotonNetwork.GameVersion = gmaeVersion; // 접속에 필요한 정보(게임 버전) 설정
        PhotonNetwork.ConnectUsingSettings(); // 설정한 정보로 마스터 서버 접속 시도
        joinBtn.interactable = false; // 룸 접속 버튼을 잠시 비활성화
        connectionStateTxt.text = "Connecting to server..."; // 접속 시도 중임을 텍스트로 표시
    }

    // '게임 참가' 버튼을 누르면 방 번호를 입력하는 UI를 표시
    public void OnInputRoomNumber() {
        enterBtn.interactable = false;
        roomNumberUI.SetActive(true);

        // TMP_InputField의 이벤트에 콜백 함수 등록
        roomNumberField.onValueChanged.AddListener(OnInputValueChanged);
    }

    // 값이 1~3자리 숫자일 때만 버튼 활성화
    void OnInputValueChanged(string text)
    {
        int value;
        bool isValid = int.TryParse(text, out value) && value >= 0 && value <= 999;
        enterBtn.interactable = isValid && text.Length <= 3;
    }

    // 방 번호를 입력한 뒤 '완료' 버튼을 누르면 번호에 해당하는 방에 입장
    public void OnEnterRoom() {
        OnConnect(roomNumberField.text);
    }

    // 마스터 서버 접속 성공 시 자동 실행
    public override void OnConnectedToMaster() {
        joinBtn.interactable = true; // 룸 접속 버튼을 활성화
        connectionStateTxt.text = "Connection success!"; // 접속 정보 표시
    }

    // 마스터 서버 접속 실패 시 자동 실행
    public override void OnDisconnected(DisconnectCause cause)
    {
        joinBtn.interactable = false; // 룸 접속 버튼을 비활성화
        connectionStateTxt.text = "Connection failure... Retry..."; // 접속 정보 표시

        PhotonNetwork.ConnectUsingSettings(); // 마스터 서버로의 재접속 시도
    }

    // 룸 접속 시도
    public void OnConnect(string roomNumber) {
        PhotonNetwork.NickName = mNickNameField.text;

        joinBtn.interactable = false; // 중복 접속 시도를 막기 위해 접속 버튼 잠시 비활성화

        if (PhotonNetwork.IsConnected) // 마스터 서버에 접속 중이라면
        {
            // 룸 접속 실행
            connectionStateTxt.text = "Entering the room..."; 
            PhotonNetwork.JoinOrCreateRoom(roomNumber, new RoomOptions {MaxPlayers=8}, TypedLobby.Default, null);
        }
        else
        {
            connectionStateTxt.text = "Disconnected from the server!";
            // 마스터 서버로의 재접속 시도
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // // (빈 방이 없어) 랜덤 룸 참가에 실패한 경우 자동 실행
    // public override void OnJoinRandomFailed(short returnCode, string message)
    // {
    //     connectionStateTxt.text = "Creating new Room...";
    //     // 최대 4명 수용 가능한 빈 방 생성
    //     PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = 4});
    // }

    // 룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom()
    {
        connectionStateTxt.text = "Enter success";
        PhotonNetwork.LoadLevel("Lobby"); // 모든 룸 참가자가 Lobby 씬을 로드하게 함
    }
}
