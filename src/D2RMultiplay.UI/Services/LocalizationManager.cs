using System.Collections.Generic;

namespace D2RMultiplay.UI.Services
{
    public static class LocalizationManager
    {
        // 5 Supported Languages: English (Default), Simplified Chinese, Traditional Chinese, Japanese, Korean
        public static readonly List<string> SupportedLanguages = new List<string> 
        { 
            "English", 
            "简体中文", 
            "繁體中文", 
            "日本語", 
            "한국어" 
        };

        private static readonly Dictionary<string, Dictionary<string, string>> _resources = new Dictionary<string, Dictionary<string, string>>
        {
            // English (en-US)
            ["English"] = new Dictionary<string, string>
            {
                ["WindowTitle"] = "D2R Multiplay",
                ["LangButton"] = "Language",
                ["GroupUserMgmt"] = "1. User & Mapping Management",
                ["GroupLaunchOps"] = "2. Launch Operations",
                ["LabelInputUser"] = "Windows Username:",
                ["LabelInputPass"] = "Password (for auto-login):",
                ["HintNoPassword"] = "Current Windows User: No password required.",
                ["LabelInputBattleTag"] = "BattleTag (Alias):",
                ["LabelInputNote"] = "Note:",
                ["BtnCreateNew"] = "Create Windows User",
                ["BtnLinkExisting"] = "Link Existing User",
                ["BtnUpdate"] = "Save Changes",
                ["BtnPickPath"] = "Browse...",
                ["BtnCreateMirror"] = "Create Mirror",
                ["BtnDeleteSysUser"] = "Delete System User",
                ["LabelCurrentAccount"] = "Current Account:",
                ["LabelGamePath"] = "Game Path:",
                ["LabelPathHint"] = "Note: Manually select unique folder or use Mirror.",
                ["LabelLanguage"] = "Language:",
                ["LabelAdminStatus_Yes"] = "🛡️ Administrator Access",
                ["LabelAdminStatus_No"] = "⚠️ Restricted Mode (No Admin)",
                ["BtnLaunchAuto"] = "One-Click Launch",
                ["BtnLaunchDirect"] = "Direct Launch",
                ["ShowPassword"] = "Show Password",
                ["LaunchHint"] = "1. [Required] Manually switch to this Windows user once (Init Env).\n2. [Suggested] Log in to Battle.net once manually.\n3. [Issue] If stuck at login, close Battle.net and retry.",
                ["BtnDelete"] = "Remove from List",
                ["BtnSave"] = "Save Path",
                ["GroupManual"] = "Manual Tools (Debug)",
                ["BtnKillBnet"] = "Kill Battle.net",
                ["BtnCleanConfig"] = "Del Config",
                ["BtnKillMutex"] = "Kill Mutex",
                ["BtnSnapshotConfig"] = "Snapshot Config",
                ["StatusReady"] = "Ready. Manage users on the left first.",
                // Secondary Windows
                ["CreateUserTitle"] = "Create New Windows User",
                ["LinkUserTitle"] = "Link Existing User",
                ["SelectUserTitle"] = "Select Windows User",
                ["LabelSelectUser"] = "Select User:",
                ["LabelVerifyPass"] = "Password (Verify):",
                ["BtnCreate"] = "Create",
                ["BtnCancel"] = "Cancel",
                ["BtnLink"] = "Link User",
                ["MsgEnterUsername"] = "Please enter a username.",
                ["MsgEnterPassword"] = "Please enter a password.",
                ["MsgSelectUser"] = "Please select or enter a user.",
                ["SuffixCurrent"] = " (Current)",
                ["MsgUserCreatedBody"] = "User {0} Created!\n\n1. Log out & Log in as '{0}' once.\n2. Open Battle.net and log in once.",
                ["TitleFirstRun"] = "First Run Setup",
                // Donation
                ["BtnSupport"] = "❤️ Donate & Boost Luck",
                ["DonationTitle"] = "Activate 'Luck Patch': Support the Author!",
                ["DonationDesc"] = "Thank you for using D2R Multiplay Agent!...\nMay your next Boss fight drop HIGH RUNES (Jah/Ber/Lo) everywhere!",
                ["LabelAlipay"] = "Alipay",
                ["LabelWeChat"] = "WeChat Pay",
                ["LabelPayPal"] = "PayPal\n(International)",
                ["LinkPayPal"] = "PayPal.Me/squareuncle",
                ["LinkDesc"] = "(Supports Credit Cards, Debit Cards & PayPal Balance)",
            },

            // Simplified Chinese (zh-CN)
            ["简体中文"] = new Dictionary<string, string>
            {
                ["WindowTitle"] = "D2R 多开工具",
                ["LangButton"] = "语言",
                ["GroupUserMgmt"] = "1. Windows 用户与映射管理",
                ["GroupLaunchOps"] = "2. 启动操作区",
                ["LabelInputUser"] = "Windows 用户名:",
                ["LabelInputPass"] = "密码 (用于自动登录):",
                ["HintNoPassword"] = "当前登录的 Windows 用户无需填写密码。",
                ["LabelInputBattleTag"] = "战网账号 (别名):",
                ["LabelInputNote"] = "备注:",
                ["BtnCreateNew"] = "新建 Windows 用户",
                ["BtnLinkExisting"] = "关联现有用户",
                ["BtnUpdate"] = "保存修改",
                ["BtnPickPath"] = "浏览...",
                ["BtnCreateMirror"] = "创建镜像",
                ["BtnDeleteSysUser"] = "删除系统用户",
                ["LabelCurrentAccount"] = "当前选中账号:",
                ["LabelGamePath"] = "游戏路径:",
                ["LabelPathHint"] = "说明: 请手动指定路径或使用镜像。",
                ["LabelLanguage"] = "语言选择:",
                ["LabelAdminStatus_Yes"] = "🛡️ 已获管理员权限",
                ["LabelAdminStatus_No"] = "⚠️ 未获管理员权限 (限制模式)",
                ["BtnLaunchAuto"] = "一键启动",
                ["BtnLaunchDirect"] = "直接启动",
                ["ShowPassword"] = "显示密码",
                ["LaunchHint"] = "1.[必须]手动切换到该用户登录一次 Windows (初始化环境)。\n2.[建议]在该用户下登录一次战网客户端(确保无异常)。\n3.[异常]若一键启动卡在登录页(请关闭战网并重试)。",
                ["BtnDelete"] = "从列表移除",
                ["BtnSave"] = "保存路径",
                ["GroupManual"] = "手动工具 (调试用)",
                ["BtnKillBnet"] = "清理战网",
                ["BtnCleanConfig"] = "删档案",
                ["BtnKillMutex"] = "杀句柄",
                ["BtnSnapshotConfig"] = "抓取配置",
                ["StatusReady"] = "就绪。请先在左侧管理用户。",
                // Secondary Windows
                ["CreateUserTitle"] = "新建 Windows 用户",
                ["LinkUserTitle"] = "关联现有用户",
                ["SelectUserTitle"] = "选择 Windows 用户",
                ["LabelSelectUser"] = "选择用户:",
                ["LabelVerifyPass"] = "密码 (验证):",
                ["BtnCreate"] = "创建",
                ["BtnCancel"] = "取消",
                ["BtnLink"] = "关联用户",
                ["MsgEnterUsername"] = "请输入用户名。",
                ["MsgEnterPassword"] = "请输入密码。",
                ["MsgSelectUser"] = "请输入或选择一个用户。",
                ["SuffixCurrent"] = " (当前用户)",
                ["MsgUserCreatedBody"] = "用户 {0} 创建成功！\n\n1. 请注销并切换到 '{0}' 登录一次（初始化环境）。\n2. 打开战网客户端并手动登录一次。",
                ["TitleFirstRun"] = "首次运行设置",
                // Donation
                ["BtnSupport"] = "❤️ 赞助 & 攒人品",
                ["DonationTitle"] = "开启 “人品补丁”：赞助作者，玄学出奇迹！",
                ["DonationDesc"] = "感谢您使用 “方砖叔暗黑多开助手” ！...\n愿你的下一场 Boss 战，满地金色光芒，乔(Jah)▽、贝(Ber)◇、罗(Lo)🔶 滚滚而来！",
                ["LabelAlipay"] = "支付宝\n(Alipay)",
                ["LabelWeChat"] = "微信支付\n(WeChat)",
                ["LabelPayPal"] = "PayPal\n(International)",
                ["LinkPayPal"] = "PayPal.Me/squareuncle",
                ["LinkDesc"] = "(支持全球信用卡、借记卡及 PayPal 余额)",
            },

            // Traditional Chinese (zh-TW)
            ["繁體中文"] = new Dictionary<string, string>
            {
                ["WindowTitle"] = "D2R 多開工具",
                ["LangButton"] = "語言",
                ["GroupUserMgmt"] = "1. Windows 使用者與映射管理",
                ["GroupLaunchOps"] = "2. 啟動操作區",
                ["LabelInputUser"] = "Windows 使用者名稱:",
                ["LabelInputPass"] = "密碼 (用於自動登入):",
                ["HintNoPassword"] = "目前登入的 Windows 使用者無需填寫密碼。",
                ["LabelInputBattleTag"] = "BattleTag (別名):",
                ["LabelInputNote"] = "備註:",
                ["BtnCreateNew"] = "新建 Windows 使用者",
                ["BtnLinkExisting"] = "連結現有使用者",
                ["BtnUpdate"] = "儲存變更",
                ["BtnPickPath"] = "瀏覽...",
                ["BtnCreateMirror"] = "建立鏡像",
                ["BtnDeleteSysUser"] = "刪除系統使用者",
                ["LabelCurrentAccount"] = "目前選定帳號:",
                ["LabelGamePath"] = "遊戲路徑:",
                ["LabelPathHint"] = "說明: 請手動指定路徑或使用鏡像。",
                ["LabelLanguage"] = "語言選擇:",
                ["LabelAdminStatus_Yes"] = "🛡️ 已獲管理員權限",
                ["LabelAdminStatus_No"] = "⚠️ 未獲管理員權限 (限制模式)",
                ["BtnLaunchAuto"] = "一鍵啟動",
                ["BtnLaunchDirect"] = "直接啟動",
                ["ShowPassword"] = "顯示密碼",
                ["LaunchHint"] = "1.[必須]手動切換到該使用者登入一次 Windows (初始化環境)。\n2.[建議]在該使用者下登入一次 Battle.net 客戶端(確保無異常)。\n3.[異常]若一鍵啟動卡在登入頁(請關閉 Battle.net 並重試)。",
                ["BtnDelete"] = "從列表移除",
                ["BtnSave"] = "儲存路徑",
                ["GroupManual"] = "手動工具 (除錯用)",
                ["BtnKillBnet"] = "清理 Battle.net",
                ["BtnCleanConfig"] = "刪檔案",
                ["BtnKillMutex"] = "殺控制代碼",
                ["BtnSnapshotConfig"] = "抓取配置",
                ["StatusReady"] = "就緒。請先在左側管理使用者。",
                // Secondary Windows
                ["CreateUserTitle"] = "新建 Windows 使用者",
                ["LinkUserTitle"] = "連結現有使用者",
                ["SelectUserTitle"] = "選擇 Windows 使用者",
                ["LabelSelectUser"] = "選擇使用者:",
                ["LabelVerifyPass"] = "密碼 (驗證):",
                ["BtnCreate"] = "建立",
                ["BtnCancel"] = "取消",
                ["BtnLink"] = "連結使用者",
                ["MsgEnterUsername"] = "請輸入使用者名稱。",
                ["MsgEnterPassword"] = "請輸入密碼。",
                ["MsgSelectUser"] = "請選擇或輸入使用者。",
                ["SuffixCurrent"] = " (目前使用者)",
                ["MsgUserCreatedBody"] = "使用者 {0} 建立成功！\n\n1. 請登出並切換到 '{0}' 登入一次（初始化環境）。\n2. 開啟 Battle.net 並手動登入一次。",
                ["TitleFirstRun"] = "首次執行設定",
                // Donation
                ["BtnSupport"] = "❤️ 贊助 & 攢人品",
                ["DonationTitle"] = "開啟 “人品補丁”：贊助作者，玄學出奇蹟！",
                ["DonationDesc"] = "感謝您使用 “方磚叔暗黑多開助手” ！...\n願你的下一場 Boss 戰，滿地金色光芒，喬(Jah)▽、貝(Ber)◇、羅(Lo)🔶 滾滾而來！",
                ["LabelAlipay"] = "支付寶\n(Alipay)",
                ["LabelWeChat"] = "微信支付\n(WeChat)",
                ["LabelPayPal"] = "PayPal\n(International)",
                ["LinkPayPal"] = "PayPal.Me/squareuncle",
                ["LinkDesc"] = "(支持全球信用卡、借記卡及 PayPal 餘額)",
            },

            // Japanese (ja-JP)
            ["日本語"] = new Dictionary<string, string>
            {
                ["WindowTitle"] = "D2R 多重起動ツール",
                ["LangButton"] = "言語",
                ["GroupUserMgmt"] = "1. ユーザー＆マッピング管理",
                ["GroupLaunchOps"] = "2. 起動オプション",
                ["LabelInputUser"] = "Windows ユーザー名:",
                ["LabelInputPass"] = "パスワード (自動ログイン用):",
                ["HintNoPassword"] = "現在のWindowsユーザーはパスワード不要です。",
                ["LabelInputBattleTag"] = "BattleTag (エイリアス):",
                ["LabelInputNote"] = "メモ:",
                ["BtnCreateNew"] = "Windows ユーザー作成",
                ["BtnLinkExisting"] = "既存ユーザーをリンク",
                ["BtnUpdate"] = "変更を保存",
                ["BtnPickPath"] = "参照...",
                ["BtnCreateMirror"] = "ミラー作成",
                ["BtnDeleteSysUser"] = "システムユーザー削除",
                ["LabelCurrentAccount"] = "選択中のアカウント:",
                ["LabelGamePath"] = "ゲームパス:",
                ["LabelPathHint"] = "注: 固有のパスを指定するか、ミラーを使用してください。",
                ["LabelLanguage"] = "言語選択:",
                ["LabelAdminStatus_Yes"] = "🛡️ 管理者権限あり",
                ["LabelAdminStatus_No"] = "⚠️ 制限モード (管理者権限なし)",
                ["BtnLaunchAuto"] = "ワンクリック起動",
                ["BtnLaunchDirect"] = "直接起動",
                ["ShowPassword"] = "パスワードを表示",
                ["LaunchHint"] = "1.[必須] 一度Windowsユーザーに手動で切り替えてログインしてください。\n2.[推奨] 問題確認のため一度手動でBattle.netにログインしてください。\n3.[例外] ログイン画面で止まる場合は、Battle.netを閉じて再試行してください。",
                ["BtnDelete"] = "リストから削除",
                ["BtnSave"] = "パスを保存",
                ["GroupManual"] = "手動ツール (デバッグ)",
                ["BtnKillBnet"] = "Battle.net 終了",
                ["BtnCleanConfig"] = "設定削除",
                ["BtnKillMutex"] = "ミューテックス削除",
                ["BtnSnapshotConfig"] = "設定スナップショット",
                ["StatusReady"] = "準備完了。左側でユーザーを管理してください。",
                // Secondary Windows
                ["CreateUserTitle"] = "新規 Windows ユーザー作成",
                ["LinkUserTitle"] = "既存ユーザーのリンク",
                ["SelectUserTitle"] = "Windows ユーザーの選択",
                ["LabelSelectUser"] = "ユーザー選択:",
                ["LabelVerifyPass"] = "パスワード (確認):",
                ["BtnCreate"] = "作成",
                ["BtnCancel"] = "キャンセル",
                ["BtnLink"] = "リンク",
                ["MsgEnterUsername"] = "ユーザー名を入力してください。",
                ["MsgEnterPassword"] = "パスワードを入力してください。",
                ["MsgSelectUser"] = "ユーザーを選択または入力してください。",
                ["SuffixCurrent"] = " (現在)",
                ["MsgUserCreatedBody"] = "ユーザー {0} を作成しました！\n\n1. 一度ログアウトし、'{0}' としてログインしてください。\n2. Battle.netを開き、一度ログインしてください。",
                ["TitleFirstRun"] = "初回セットアップ",
                // Donation
                ["BtnSupport"] = "❤️ 寄付 & 運気アップ",
                ["DonationTitle"] = "「ラックパッチ」を有効化：作者をサポート！",
                ["DonationDesc"] = "D2R Multiplay Agentをご利用いただきありがとうございます！\n次のボス戦で、Jah/Ber/Loルーンがドロップしますように！",
                ["LabelAlipay"] = "Alipay",
                ["LabelWeChat"] = "WeChat Pay",
                ["LabelPayPal"] = "PayPal\n(International)",
                ["LinkPayPal"] = "PayPal.Me/squareuncle",
                ["LinkDesc"] = "(クレジットカード、デビットカード、PayPal残高対応)",
            },

            // Korean (ko-KR)
            ["한국어"] = new Dictionary<string, string>
            {
                ["WindowTitle"] = "D2R 다중 실행 도구",
                ["LangButton"] = "언어",
                ["GroupUserMgmt"] = "1. 사용자 및 매핑 관리",
                ["GroupLaunchOps"] = "2. 실행 작업",
                ["LabelInputUser"] = "Windows 사용자 이름:",
                ["LabelInputPass"] = "비밀번호 (자동 로그인용):",
                ["HintNoPassword"] = "현재 로그인된 Windows 사용자는 비밀번호가 필요 없습니다.",
                ["LabelInputBattleTag"] = "배틀태그 (별칭):",
                ["LabelInputNote"] = "메모:",
                ["BtnCreateNew"] = "Windows 사용자 생성",
                ["BtnLinkExisting"] = "기존 사용자 연결",
                ["BtnUpdate"] = "변경 사항 저장",
                ["BtnPickPath"] = "찾아보기...",
                ["BtnCreateMirror"] = "미러 생성",
                ["BtnDeleteSysUser"] = "시스템 사용자 삭제",
                ["LabelCurrentAccount"] = "현재 계정:",
                ["LabelGamePath"] = "게임 경로:",
                ["LabelPathHint"] = "참고: 고유 경로를 수동으로 지정하거나 미러를 사용하세요.",
                ["LabelLanguage"] = "언어 선택:",
                ["LabelAdminStatus_Yes"] = "🛡️ 관리자 권한 확보",
                ["LabelAdminStatus_No"] = "⚠️ 제한 모드 (관리자 권한 없음)",
                ["BtnLaunchAuto"] = "원클릭 실행",
                ["BtnLaunchDirect"] = "직접 실행",
                ["ShowPassword"] = "비밀번호 표시",
                ["LaunchHint"] = "1. [필수] 해당 Windows 사용자로 한 번 수동 로그인하십시오 (환경 초기화).\n2. [권장] 문제가 없는지 확인하기 위해 배틀넷에 한 번 수동으로 로그인하십시오.\n3. [문제] 로그인 화면에서 멈추면 배틀넷을 닫고 다시 시도하십시오.",
                ["BtnDelete"] = "목록에서 제거",
                ["BtnSave"] = "경로 저장",
                ["GroupManual"] = "수동 도구 (디버그)",
                ["BtnKillBnet"] = "배틀넷 종료",
                ["BtnCleanConfig"] = "설정 삭제",
                ["BtnKillMutex"] = "뮤텍스 제거",
                ["BtnSnapshotConfig"] = "설정 스냅샷",
                ["StatusReady"] = "준비 완료. 왼쪽에서 사용자를 관리하세요.",
                // Secondary Windows
                ["CreateUserTitle"] = "새 Windows 사용자 만들기",
                ["LinkUserTitle"] = "기존 사용자 연결",
                ["SelectUserTitle"] = "Windows 사용자 선택",
                ["LabelSelectUser"] = "사용자 선택:",
                ["LabelVerifyPass"] = "비밀번호 (확인):",
                ["BtnCreate"] = "만들기",
                ["BtnCancel"] = "취소",
                ["BtnLink"] = "연결",
                ["MsgEnterUsername"] = "사용자 이름을 입력하세요.",
                ["MsgEnterPassword"] = "비밀번호를 입력하세요.",
                ["MsgSelectUser"] = "사용자를 선택하거나 입력하세요.",
                ["SuffixCurrent"] = " (현재)",
                ["MsgUserCreatedBody"] = "사용자 {0} 생성됨!\n\n1. 로그아웃 후 '{0}'(으)로 한 번 로그인하십시오.\n2. 배틀넷을 열고 한 번 로그인하십시오.",
                ["TitleFirstRun"] = "최초 실행 설정",
                // Donation
                ["BtnSupport"] = "❤️ 후원 & 행운 상승",
                ["DonationTitle"] = "'행운 패치' 활성화: 제작자 후원!",
                ["DonationDesc"] = "D2R Multiplay Agent를 사용해 주셔서 감사합니다!...\n다음 보스전에서 자(Jah)/베르(Ber)/로(Lo) 룬이 쏟아지길 기원합니다!",
                ["LabelAlipay"] = "Alipay",
                ["LabelWeChat"] = "WeChat Pay",
                ["LabelPayPal"] = "PayPal\n(International)",
                ["LinkPayPal"] = "PayPal.Me/squareuncle",
                ["LinkDesc"] = "(신용카드, 직불카드 및 PayPal 잔액 지원)",
            }
        };

        public static string CurrentLanguage { get; set; } = "English";

        public static string GetText(string key)
        {
            return GetText(key, CurrentLanguage);
        }

        public static string GetText(string key, string language)
        {
            if (_resources.TryGetValue(language, out var dict))
            {
                if (dict.TryGetValue(key, out var text))
                {
                    return text;
                }
            }
            
            // Fallback to English
            if (_resources["English"].TryGetValue(key, out var engText))
            {
                return engText;
            }

            return key; // Fallback to Key itself
        }
    }
}
