using Model;
using OCUnion;
using OCUnion.Transfer;
using OCUnion.Transfer.Model;
using RimWorldOnlineCity.Services;
using RimWorldOnlineCity.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimWorldOnlineCity
{
    public class PanelProfilePlayer : DialogControlBase
    {
        // ---------- Размеры раскладки ----------
        private const float Pad = 30f;            // отступ слева
        private const float ColumnGap = 15f;      // зазор между колонкой полей и колонкой аватара
        private const float AvatarSize = 256f;
        private const float AvatarBtnW = 200f;
        private const float RowH = 25f;           // высота строки (поле ввода, чекбокс)
        private const float FieldW = 250f;        // ширина полей ввода
        private const float BtnH = 30f;
        private const float SaveBtnW = 200f;
        private const float MinAboutH = 60f;      // «Обо мне»: минимальная высота
        private const float MaxAboutH = 220f;     // «Обо мне»: максимальная высота (кнопка «Сохранить» идёт сразу под ним)

        private const int MinDelaySaveGame = 5;   // минимальный интервал сохранений, минут
        private const int MaxDelayDigits = 5;
        private const int MaxTextLength = 1000;
        private const int LogKeyAboutBox = 0x0C170A01;

        private bool Inited;
        private Player Player;

        private bool Input_EnablePVP;
        private string Input_DelaySaveGame;
        private string Input_DiscordUserName;
        private string Input_EMail;
        private TextBox Input_AboutMyTextBox = new TextBox() { Editable = true };

        public void Init()
        {
            Player = SessionClientController.My;
            if (Player == null) return; // игрок ещё не загружен, повторим в следующем кадре

            Input_EnablePVP = Player.EnablePVP;
            Input_DelaySaveGame = SessionClientController.Data.DelaySaveGame.ToString();
            Input_DiscordUserName = Player.DiscordUserName ?? "";
            Input_EMail = Player.EMail ?? "";
            Input_AboutMyTextBox.Text = Player.AboutMyText ?? "";

            // флаг ставим только после успешной инициализации
            Inited = true;
        }


        public bool Validate()
        {
            if (!int.TryParse(Input_DelaySaveGame, out int delaySaveGame)
                || delaySaveGame < MinDelaySaveGame
                ) return false;

            return true;
        }

        public void Save()
        {
            if (!Validate())
            {
                Load();
                return;
            }
            var delaySaveGame = int.Parse(Input_DelaySaveGame);

            var spi = new SetPlayerInfo(SessionClient.Get);
            spi.GenerateRequestAndDoJob(new ModelPlayerInfo()
            {
                DelaySaveGame = delaySaveGame,
                EnablePVP = Input_EnablePVP,
                DiscordUserName = Input_DiscordUserName,
                EMail = Input_EMail,
                AboutMyTextBox = Input_AboutMyTextBox.Text,
            });

            Load();
        }

        public void Load()
        {
            var connect = SessionClient.Get;
            var serverInfo = connect.GetInfo(ServerInfoType.Full);
            SessionClientController.SetFullInfo(serverInfo);
            Init();
        }

        public void Drow(Rect inRect)
        {
            if (!Inited) Init();
            if (!Inited) return;

            // Запоминаем состояние GUI и в конце возвращаем, чтобы не ломать соседние панели
            var oldFont = Text.Font;
            var oldAnchor = Text.Anchor;
            var oldWrap = Text.WordWrap;
            var oldColor = GUI.color;
            try
            {
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.UpperLeft;
                Text.WordWrap = true;
                GUI.color = Color.white;

                DrawForm(inRect);
            }
            finally
            {
                Text.Font = oldFont;
                Text.Anchor = oldAnchor;
                Text.WordWrap = oldWrap;
                GUI.color = oldColor;
            }
        }

        private void DrawForm(Rect inRect)
        {
            // Две колонки: слева поля ввода, справа аватар. Друг на друга они не заходят.
            float leftX = inRect.x + Pad;
            float rightX = inRect.xMax - AvatarSize - 5f;
            float leftW = Mathf.Max(120f, rightX - leftX - ColumnGap);
            float fieldW = Mathf.Min(FieldW, leftW);
            float y = inRect.y;

            /// Заголовок
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(inRect.x, y, leftW + Pad, 32f),
                "OCity_PlayerClient_Settings".Translate().ToString() + " " + Player.Login);
            Text.Font = GameFont.Small;
            y += 36f;

            /// Я учавствую в PVP
            if (SessionClientController.Data.GeneralSettings.EnablePVP || Player.EnablePVP)
            {
                bool locked = SessionClientController.Data.TimeChangeEnablePVP >= DateTime.UtcNow;

                Widgets.CheckboxLabeled(new Rect(leftX, y, fieldW, RowH),
                    "OCity_PlayerClient_InvolvedInPVP".Translate(), ref Input_EnablePVP, locked, null, null, true);
                y += RowH + 2f;

                if (locked)
                {
                    Input_EnablePVP = Player.EnablePVP;
                    y += DrawWrappedLabel(leftX, y, leftW,
                        "OCity_PlayerClient_CanChange".Translate(SessionClientController.Data.TimeChangeEnablePVP.ToGoodUtcString()));
                }
                else
                {
                    y += DrawWrappedLabel(leftX, y, leftW, "OCity_PlayerClient_CanChangeNow".Translate());
                }
                y += 8f;
            }

            /// Аватарка (правая колонка)
            DrawAvatarColumn(new Rect(rightX, inRect.y + 5f, AvatarSize, AvatarSize));

            /// Интервал сохранений в минутах (только цифры)
            y += DrawWrappedLabel(leftX, y, leftW, "OCity_PlayerClient_SaveInterval".Translate()) + 2f;
            DrawIntervalField(new Rect(leftX, y, fieldW, RowH));
            y += RowH + 8f;

            /// Мой дискорд
            y = DrawFieldRow(y, leftX, leftW, fieldW, "OCity_PlayerClient_Discord".Translate(), ref Input_DiscordUserName);

            /// Почта
            y = DrawFieldRow(y, leftX, leftW, fieldW, "OCity_PlayerClient_Email".Translate(), ref Input_EMail);

            /// Обо мне + кнопка «Сохранить»
            y += DrawWrappedLabel(leftX, y, leftW, "OCity_PlayerClient_AboutMyself".Translate()) + 2f;

            float aboutH = Mathf.Clamp(inRect.yMax - y - (BtnH + 15f), MinAboutH, MaxAboutH);
            var aboutRect = new Rect(leftX, y, leftW, aboutH);
            var saveRect = new Rect(leftX, aboutRect.yMax + 8f, SaveBtnW, BtnH);

            // Кнопку рисуем ДО поля «Обо мне»: даже если оно упадёт с исключением,
            // кнопка «Сохранить» останется на экране (причина будет в Player.log).
            if (Widgets.ButtonText(saveRect, "OCity_PlayerClient_Save".Translate()))
            {
                if (Validate())
                {
                    Save();
                }
                else
                {
                    // введённые данные не сбрасываем, поле интервала подсвечено красным
                    Find.WindowStack.Add(new Dialog_MessageBox("OCity_Error".Translate()));
                }
                return; // после Save данные перечитаны, остальное нарисуем в следующем кадре
            }

            try
            {
                Input_AboutMyTextBox.Drow(aboutRect);
            }
            catch (Exception ex)
            {
                Log.ErrorOnce("PanelProfilePlayer: failed to draw the 'about me' text box: " + ex, LogKeyAboutBox);
            }
        }

        private void DrawAvatarColumn(Rect avatarRect)
        {
            var login = Player.Login;

            var iconImage = GeneralTexture.Get.ByName("pl_" + login);
            if (iconImage != null) GUI.DrawTexture(avatarRect, iconImage);

            var btnRect = new Rect(avatarRect.x + (avatarRect.width - AvatarBtnW) / 2f, avatarRect.yMax + 5f, AvatarBtnW, BtnH);
            if (Widgets.ButtonText(btnRect, "OCity_SetAvatar".Translate()))
            {
                Find.WindowStack.Add(new Dialog_InputImage()
                {
                    SelectImageAction = (img, data) =>
                    {
                        SessionClientController.Command((connect) =>
                        {
                            connect.FileSharingUpload(FileSharingCategory.PlayerIcon, login, data);
                            var p = connect.FileSharingDownload(FileSharingCategory.PlayerIcon, login);

                            GeneralTexture.Clear();

                            var msg = data?.Length > 0 && p?.Data?.Length > 0
                                ? "OCity_Successfully".Translate() : "OCity_Error".Translate();
                            Find.WindowStack.Add(new Dialog_MessageBox(msg));
                        });
                    }
                });
            }
        }

        /// <summary>Поле интервала: принимает только цифры, при неверном значении подсвечивается красным.</summary>
        private void DrawIntervalField(Rect rect)
        {
            if (!Validate()) GUI.color = new Color(1f, 0.45f, 0.45f);
            var text = Widgets.TextField(rect, Input_DelaySaveGame ?? "");
            GUI.color = Color.white;

            if (text != Input_DelaySaveGame && text.Length <= MaxDelayDigits && IsDigitsOnly(text))
            {
                Input_DelaySaveGame = text;
            }
        }

        /// <summary>Подпись + однострочное поле ввода. Возвращает Y следующей строки.</summary>
        private static float DrawFieldRow(float y, float x, float labelW, float fieldW, TaggedString label, ref string value)
        {
            y += DrawWrappedLabel(x, y, labelW, label) + 2f;

            var text = Widgets.TextField(new Rect(x, y, fieldW, RowH), value ?? "");
            if (text.Length > MaxTextLength) text = text.Substring(0, MaxTextLength);
            value = text;

            return y + RowH + 8f;
        }

        /// <summary>Подпись с переносом строк. Возвращает занятую высоту.</summary>
        private static float DrawWrappedLabel(float x, float y, float width, TaggedString label)
        {
            float h = Mathf.Max(RowH, Text.CalcHeight(label, width));
            Widgets.Label(new Rect(x, y, width, h), label);
            return h;
        }

        private static bool IsDigitsOnly(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] < '0' || s[i] > '9') return false;
            }
            return true;
        }
    }
}