using RimWorld;
using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace PsychicBondTweaks
{
    public class Listing_GUI : Listing_Standard
    {
        private const float ScrollAreaWidth = 24f;
        private float paddingRight;
        private float paddingLeft;
        private static Rect viewRect;
        private static int elementsGap = 0;


        public void BeginScrollView(Rect rect, ref Vector2 scrollPosition, ref Rect viewRect)
        {
            maxOneColumn = true;

            if (viewRect == default) { viewRect = new Rect(rect.x, rect.y, rect.width - ScrollAreaWidth, 99999f); }

            Widgets.BeginScrollView(rect, ref scrollPosition, viewRect);

            Begin(viewRect);
        }

        public void BeginScrollView(Rect rect, ref Vector2 scrollPosition)
        {
            BeginScrollView(rect, ref scrollPosition, ref viewRect);
        }

        public void EndScrollView(ref Rect viewRect)
        {
            viewRect.height = CurHeight;
            End();
            Widgets.EndScrollView();
        }

        public void EndScrollView()
        {
            EndScrollView(ref viewRect);
        }

        public void SetPadding(float _paddingLeft, float _paddingRight)
        {
            paddingLeft = _paddingLeft;
            paddingRight = _paddingRight;
        }

        public void ResetPadding()
        {
            paddingRight = 0;
            paddingLeft = 0;
        }

        public void SetElementsGap(int gap)
        {
            elementsGap = gap;
        }

        public Rect GetRect(float height, bool addPadding = true, float widthPct = 1f)
        {
            NewColumnIfNeeded(height);
            Rect result = new Rect(curX, curY, ColumnWidth * widthPct, height);
            if (addPadding)
            {
                result.x += paddingLeft;
                result.width -= paddingLeft + paddingRight;
            }
            curY += height;
            return result;
        }

        public void Title(string labelKey, bool showDescription = true)
        {
            Text.Font = GameFont.Medium;
            GUI.color = Color.white;

            string label = labelKey.PBTranslate();
            Rect rect = GetRect(Text.CalcHeight(label, ColumnWidth), false);
            Widgets.Label(rect, label);

            if (showDescription)
            {
                ShowDescription(labelKey, addPadding: false);
            }

            Text.Font = GameFont.Small;

            GUI.color = Color.white;

            GapLine();
            Gap(10);
            Gap(elementsGap);
        }

        public void SliderLabeled(string labelKey, ref float value, float min, float max, bool percent, string searchReplace = "", bool showDescription = true)
        {
            if (value < min || value > max)
            {
                value = min;
            }
            var startHeight = CurHeight;

            var rect = GetRect(Text.LineHeight + verticalSpacing);

            Text.Font = GameFont.Small;
            GUI.color = Color.white;

            var savedAnchor = Text.Anchor;

            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect, labelKey.PBTranslate(searchReplace));

            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(rect, $"{value}{(percent ? "%" : "")}");

            Text.Anchor = savedAnchor;

            if (showDescription)
            {
                ShowDescription(labelKey, searchReplace);
            }

            Gap(6);

            value = (int)Slider(value, min, max);
            rect = GetRect(0, false);
            rect.height = CurHeight - startHeight;
            rect.y -= rect.height;

            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
                /*if (!tooltip.NullOrEmpty())
                    TooltipHandler.TipRegion(rect, tooltip);*/
            }

            Gap(6);
            Gap(elementsGap);
        }

        public void SliderLabeled(string labelKey, ref int value, int min, int max, bool percent, string searchReplace = "", bool showDescription = true)
        {
            var floatValue = (float)value;
            SliderLabeled(labelKey, ref floatValue, min, max, percent, searchReplace, showDescription);
            value = (int)floatValue;
        }

        new public float Slider(float val, float min, float max)
        {
            Rect rect = GetRect(22f);
            rect.x -= 6;
            rect.width += 6;
            float result = Widgets.HorizontalSlider_NewTemp(rect, val, min, max);
            Gap(verticalSpacing);
            return result;
        }

        public void CheckboxLabeled(string labelKey, ref bool value, Action onChange = null, string serachReplace = "", bool showDescription = true)
        {
            var startHeight = CurHeight;

            Rect rect = GetRect(Text.LineHeight + verticalSpacing);

            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            var oldValue = value;
            Widgets.CheckboxLabeled(rect, labelKey.PBTranslate(serachReplace), ref value);
            if (onChange != null && value != oldValue)
            {
                onChange();
            }

            if (showDescription)
            {
                ShowDescription(labelKey, serachReplace);
            }

            rect = GetRect(0, false);
            rect.height = CurHeight - startHeight;
            rect.y -= rect.height;

            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
                /*if (!tooltip.NullOrEmpty())
                    TooltipHandler.TipRegion(rect, tooltip);*/
            }

            Gap(6);
            Gap(elementsGap);
        }

        private Rect ShowDescription(string labelKey, string serachReplace = "", bool addPadding = true)
        {
            Text.Font = GameFont.Tiny;
            ColumnWidth -= 34;
            GUI.color = Color.gray;

            string description = $"{labelKey}_desc".PBTranslate(serachReplace);
            Rect rect = GetRect(Text.CalcHeight(description, ColumnWidth), addPadding);
            Widgets.Label(rect, description);

            ColumnWidth += 34;
            Text.Font = GameFont.Small;

            return rect;
        }

        public void ValueLabeled<T>(string name, bool useValueForExplain, ref T value, string tooltip = null)
        {
            var startHeight = CurHeight;

            var rect = GetRect(Text.LineHeight + verticalSpacing);

            Text.Font = GameFont.Small;
            GUI.color = Color.white;

            var savedAnchor = Text.Anchor;

            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect, (name + "Title").Translate());

            Text.Anchor = TextAnchor.MiddleRight;
            var valueLabel = typeof(T).Name + "Option" + value.ToString();
            if (typeof(T).IsEnum)
                Widgets.Label(rect, valueLabel.Translate());
            else
                Widgets.Label(rect, value.ToString());

            Text.Anchor = savedAnchor;

            var key = (useValueForExplain ? valueLabel : name) + "Explained";
            if (key.CanTranslate())
            {
                Text.Font = GameFont.Tiny;
                ColumnWidth -= 34;
                GUI.color = Color.gray;
                _ = Label(key.Translate());
                ColumnWidth += 34;
                Text.Font = GameFont.Small;
            }

            rect = GetRect(0);
            rect.height = CurHeight - startHeight;
            rect.y -= rect.height;
            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
                if (!tooltip.NullOrEmpty())
                    TooltipHandler.TipRegion(rect, tooltip);

                if (Event.current.isMouse && Event.current.button == 0 && Event.current.type == EventType.MouseDown)
                {
                    var keys = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
                    for (var i = 0; i < keys.Length; i++)
                    {
                        var newValue = keys[(i + 1) % keys.Length];
                        if (keys[i].ToString() == value.ToString())
                        {
                            value = newValue;
                            break;
                        }
                    }
                    Event.current.Use();
                }
            }

            Gap(6);
            Gap(elementsGap);
        }

        public void ColorEntry(string label, ref string buffer, ref Color original)
        {
            var rect = GetRect(Text.LineHeight);
            var rectLeft = rect.LeftHalf().Rounded();
            var rectRight = rect.RightHalf().Rounded();
            var rectEntry = rectRight.LeftHalf().Rounded();
            var rectPreview = rectRight.RightHalf().Rounded();
            Widgets.Label(rectLeft, label);

            Widgets.DrawBoxSolid(rectPreview, original);
            Widgets.DrawBox(rectPreview);

            if (buffer == null) { buffer = ColorUtility.ToHtmlStringRGB(original); }

            buffer = (rect.height <= 30f ? Widgets.TextField(rectEntry, buffer) : Widgets.TextArea(rectEntry, buffer)).ToUpper();

            var color = original;
            var valid = buffer.Length == 6 && ColorUtility.TryParseHtmlString("#" + buffer, out color);

            if (!valid)
            {
                var guiColor = GUI.color;
                GUI.color = Color.red;
                Widgets.DrawBox(rectEntry);
                GUI.color = guiColor;
            }

            original = valid ? color : original;
        }
    }
}
